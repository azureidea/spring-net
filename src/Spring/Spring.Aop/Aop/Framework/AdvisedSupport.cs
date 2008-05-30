#region License

/*
 * Copyright � 2002-2005 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#region Imports

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

using AopAlliance.Aop;
using AopAlliance.Intercept;
using Spring.Aop;
using Spring.Aop.Support;
using Spring.Aop.Target;
using Spring.Util;
using Spring.Proxy;

#endregion

namespace Spring.Aop.Framework
{
    /// <summary>
    /// Superclass for AOP proxy configuration managers.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Instances of this class are not themselves AOP proxies, but
    /// subclasses of this class are normally factories from which AOP proxy
    /// instances are obtained directly.
    /// </p>
    /// <p>
    /// This class frees subclasses of the housekeeping of
    /// <see cref="AopAlliance.Intercept.IInterceptor"/> and
    /// <see cref="Spring.Aop.IAdvisor"/> instances, but doesn't actually
    /// implement proxy creation methods, the functionality for which
    /// is provided by subclasses.
    /// </p>
    /// </remarks>
    /// <author>Rod Johnson</author>
    /// <author>Aleksandar Seovic (.NET)</author>
    /// <version>$Id: AdvisedSupport.cs,v 1.36 2008/01/14 20:49:47 oakinger Exp $</version>
    /// <seealso cref="Spring.Aop.Framework.IAopProxy"/>
    [Serializable]
    public class AdvisedSupport : ProxyConfig, IAdvised
    {
        #region Fields

        /// <summary>The list of advice.</summary>
        /// <remarks>
        /// <p>
        /// If an <see cref="AopAlliance.Intercept.IInterceptor"/> is added, it
        /// will be wrapped in an advice before being added to this list. 
        /// </p>
        /// </remarks>
        private IList _advisors = new ArrayList();

        /// <summary> 
        /// Array updated on changes to the advisors list, which is easier to
        /// manipulate internally
        /// </summary>
        private IAdvisor[] _advisorsArray = new IAdvisor[] {};

        /// <summary> 
        /// List of introductions. 
        /// </summary>
        private IList _introductions = new ArrayList();

        /// <summary> 
        /// Array updated on changes to the advisors list, which is easier to
        /// manipulate internally
        /// </summary>
        private IIntroductionAdvisor[] _introductionsArray
            = new IIntroductionAdvisor[] {};

        /// <summary>
        /// Interface map specifying which object should interface methods be
        /// delegated to.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If entry value is <cref lang="null"/> methods should be delegated
        /// to the target object.
        /// </p>
        /// </remarks>
        private IDictionary interfaceMap = new ListDictionary();

        /// <summary>
        /// The <see cref="Spring.Aop.ITargetSource"/> for this instance.
        /// </summary>
        protected internal ITargetSource m_targetSource = EmptyTargetSource.Empty;

        /// <summary>
        /// Set to <see langword="true"/> when the first AOP proxy has been
        /// created, meaning that we must track advice changes via the
        /// OnAdviceChange() callback.
        /// </summary>
        private bool isActive;

        private Type proxyType;
        private ConstructorInfo proxyConstructor;

        /// <summary>
        /// The list of <see cref="Spring.Aop.Framework.AdvisedSupport"/> event listeners.
        /// </summary>
        private IList listeners = new ArrayList();

        /// <summary>
        /// The advisor chain factory.
        /// </summary>
        private IAdvisorChainFactory advisorChainFactory;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="Spring.Aop.Framework.AdvisedSupport"/> class using the
        /// default advisor chain factory.
        /// </summary>
        public AdvisedSupport()
        {
            AdvisorChainFactory = new HashtableCachingAdvisorChainFactory();
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="Spring.Aop.Framework.AdvisedSupport"/> class.
        /// </summary>
        /// <param name="interfaces">The interfaces that are to be proxied.</param>
        /// <exception cref="Spring.Aop.Framework.AopConfigException">
        /// If this 
        /// </exception>
        public AdvisedSupport(Type[] interfaces) : this()
        {
            if (interfaces != null)
            {
                foreach (Type intf in interfaces)
                {
                    AddInterfaceInternal(intf);
                }
            }
        }

        #endregion

        #region IAdvised implementation

        /// <summary>
        /// Gets and sets the
        /// <see cref="Spring.Aop.Framework.IAdvisorChainFactory"/>
        /// implementation that will be used to get the interceptor
        /// chains for the advised
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Target"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Spring.Aop.Framework.IAdvisorChainFactory"/>
        /// implementation that will be used to get the interceptor
        /// chains for the advised
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Target"/>.
        /// </value>
        public virtual IAdvisorChainFactory AdvisorChainFactory
        {
            get
            {
                lock(this.SyncRoot)
                {
                    return this.advisorChainFactory;
                }
            }
            set
            {
                lock(this.SyncRoot)
                {
                    if (this.advisorChainFactory != null)
                    {
                        RemoveListener(this.advisorChainFactory);
                    }
                    this.advisorChainFactory = value;
                    AddListener(this.advisorChainFactory);
                }
            }
        }

        /// <summary>
        /// Returns the current <see cref="Spring.Aop.ITargetSource"/> used
        /// by this <see cref="Spring.Aop.Framework.IAdvised"/> object.
        /// </summary>
        /// <value>
        /// The <see cref="Spring.Aop.ITargetSource"/> used by this
        /// <see cref="Spring.Aop.Framework.IAdvised"/> object.
        /// </value>
        /// <see cref="Spring.Aop.Framework.IAdvised.TargetSource"/>
        public ITargetSource TargetSource
        {
            get { return this.m_targetSource; }
            set
            {
                bool initialized = !(this.m_targetSource is EmptyTargetSource);
                this.m_targetSource = value;
                
                if (this.m_targetSource != null && !initialized && interfaceMap.Count == 0)
                {
                    Type[] interfaces = ReflectionUtils.GetInterfaces(this.m_targetSource.TargetType);
                    foreach (Type intf in interfaces)
                    {    
                        AddInterfaceInternal(intf);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a boolean specifying if this <see cref="IAdvised"/>
        /// instance can be serialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can be serialized, <c>false</c> otherwise.
        /// </value>
        public bool IsSerializable
        {
            get
            {
                bool canBeSerialized = TargetSource.TargetType.IsSerializable;
                if (canBeSerialized)
                {
                    for (int i = 0; canBeSerialized && i < _advisorsArray.Length; i++)
                    {
                        IAdvisor advisor = _advisorsArray[i];
                        canBeSerialized = advisor.GetType().IsSerializable
                                          && advisor.Advice.GetType().IsSerializable;
                    }
                    for (int i = 0; canBeSerialized && i < _introductionsArray.Length; i++)
                    {
                        IIntroductionAdvisor advisor = _introductionsArray[i];
                        canBeSerialized = advisor.GetType().IsSerializable
                                          && advisor.Advice.GetType().IsSerializable;
                    }
                }

                return canBeSerialized;
            }
        }

        /// <summary>
        /// Returns the collection of interface <see cref="System.Type"/>s
        /// to be (or that are being) proxied by this proxy.
        /// </summary>
        /// <value>
        /// The collection of interface <see cref="System.Type"/>s
        /// to be (or that are being) proxied by this proxy.
        /// </value>
        /// <seealso cref="Spring.Aop.Framework.IAdvised.Interfaces"/>
        public virtual Type[] Interfaces
        {
            get
            {
                lock(this.SyncRoot)
                {
                    Type[] proxiedInterfaces = new Type[this.interfaceMap.Keys.Count];
                    this.interfaceMap.Keys.CopyTo(proxiedInterfaces, 0);
                    return proxiedInterfaces;
                }
            }
            set
            {
                lock(this.SyncRoot)
                {
                    this.interfaceMap.Clear();
                    for (int i = 0; i < value.Length; i++)
                    {
                        AddInterfaceInternal(value[i]);
                    }
                    InterfacesChanged();
                }
            }
        }

        /// <summary>
        /// Returns the mapping of the proxied interface
        /// <see cref="System.Type"/>s to their delegates.
        /// </summary>
        /// <value>
        /// The mapping of the proxied interface
        /// <see cref="System.Type"/>s to their delegates.
        /// </value>
        /// <seealso cref="Spring.Aop.Framework.IAdvised.InterfaceMap"/>
        public virtual IDictionary InterfaceMap
        {
            get
            {
                lock(this.SyncRoot)
                {
                    return new Hashtable(this.interfaceMap);
                }
            }
        }

        /// <summary>
        /// Is the supplied <paramref name="intf"/> (interface)
        /// <see cref="System.Type"/> proxied?
        /// </summary>
        /// <param name="intf">
        /// The interface <see cref="System.Type"/> to test.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the supplied <paramref name="intf"/>
        /// (interface) <see cref="System.Type"/> is proxied;
        /// <see langword="false"/> if not or the supplied
        /// <paramref name="intf"/> is <cref lang="null"/>.
        /// </returns>
        /// <seealso cref="Spring.Aop.Framework.IAdvised.IsInterfaceProxied"/>
        public virtual bool IsInterfaceProxied(Type intf)
        {
            if (intf != null)
            {
                lock(this.SyncRoot)
                {
                    foreach (Type proxyInterface in this.interfaceMap.Keys)
                    {
                        if (intf.IsAssignableFrom(proxyInterface))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the collection of <see cref="Spring.Aop.IAdvisor"/>
        /// instances that have been applied to this proxy.
        /// </summary>
        /// <value>
        /// The collection of <see cref="Spring.Aop.IAdvisor"/>
        /// instances that have been applied to this proxy.
        /// </value>
        /// <seealso cref="Spring.Aop.Framework.IAdvised.Advisors"/>
        public virtual IAdvisor[] Advisors
        {
            get
            {
                lock(this.SyncRoot)
                {
                    return (IAdvisor[]) this._advisorsArray.Clone();
                }
            }
        }

        /// <summary>
        /// Returns the collection of <see cref="Spring.Aop.IIntroductionAdvisor"/>
        /// instances that have been applied to this proxy.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Will never return <cref lang="null"/>, but may return an
        /// empty array (in the case where no
        /// <see cref="Spring.Aop.IIntroductionAdvisor"/> instances have been
        /// applied to this proxy).
        /// </p>
        /// </remarks>
        /// <value>
        /// The collection of <see cref="Spring.Aop.IIntroductionAdvisor"/>
        /// instances that have been applied to this proxy.
        /// </value>
        /// <seealso cref="Spring.Aop.Framework.IAdvised.Introductions"/>
        public virtual IIntroductionAdvisor[] Introductions
        {
            get
            {
                lock(this.SyncRoot)
                {
                    return (IIntroductionAdvisor[]) this._introductionsArray.Clone();
                }
            }
        }

        /// <summary>
        /// Adds the supplied <paramref name="advice"/> to the end (or tail)
        /// of the advice (interceptor) chain.
        /// </summary>
        /// <param name="advice">
        /// The <see cref="AopAlliance.Aop.IAdvice"/> to be added.
        /// </param>
        /// <seealso cref="Spring.Aop.Support.DefaultPointcutAdvisor"/>
        /// <seealso cref="Spring.Aop.Framework.IAdvised.AddAdvice(int,IAdvice)"/>
        public void AddAdvice(IAdvice advice)
        {
            //int position = this._advisors != null ? this._advisors.Count : 0;
            AddAdvice(-1, advice);
        }

        /// <summary>
        /// Adds the supplied <paramref name="advice"/> to the supplied
        /// <paramref name="position"/> in the advice (interceptor) chain.
        /// </summary>
        /// <param name="position">
        /// The zero (0) indexed position (from the head) at which the
        /// supplied <paramref name="advice"/> is to be inserted into the
        /// advice (interceptor) chain.
        /// </param>
        /// <param name="advice">
        /// The <see cref="AopAlliance.Aop.IAdvice"/> to be added.
        /// </param>
        /// <exception cref="Spring.Aop.Framework.AopConfigException">
        /// If the supplied <paramref name="advice"/> is <cref lang="null"/>;
        /// or is not an <see cref="AopAlliance.Intercept.IMethodInterceptor"/>
        /// reference; or if the supplied <paramref name="advice"/> is a
        /// <see cref="Spring.Aop.IIntroductionInterceptor"/>.
        /// </exception>
        /// <seealso cref="Spring.Aop.Support.DefaultPointcutAdvisor"/>
        /// <seealso cref="IAdvised.AddAdvice(IAdvice)"/>
        public void AddAdvice(int position, IAdvice advice)
        {
            if (advice is IInterceptor && !(advice is IMethodInterceptor))
            {
                throw new AopConfigException(
                    GetType().FullName + " can only handle AOP Alliance IMethodInterceptor advice.");
            }
            if (advice is IIntroductionInterceptor)
            {
                throw
                    new AopConfigException(
                        "IIntroductionInterceptors may only be added as part of IIntroductionAdvisor.");
            }

            AddAdvisor(position, new DefaultPointcutAdvisor(advice));
        }

        /// <summary> 
        /// Return the index (0 based) of the supplied
        /// <see cref="Spring.Aop.IAdvisor"/> in the interceptor
        /// (advice) chain for this proxy.
        /// </summary>
        /// <param name="advisor">
        /// The <see cref="Spring.Aop.IAdvisor"/> to search for.
        /// </param>
        /// <returns>
        /// The zero (0) based index of this advisor, or -1 if the
        /// supplied <paramref name="advisor"/> is not an advisor for this
        /// proxy.
        /// </returns>
        public virtual int IndexOf(IAdvisor advisor)
        {
            lock(this.SyncRoot)
            {
                return IndexOfInternal(advisor);
            }
        }

        /// <summary> 
        /// Return the index (0 based) of the supplied
        /// <see cref="Spring.Aop.IIntroductionAdvisor"/> in the introductions
        /// for this proxy.
        /// </summary>
        /// <param name="advisor">
        /// The <see cref="Spring.Aop.IIntroductionAdvisor"/> to search for.
        /// </param>
        /// <returns>
        /// The zero (0) based index of this advisor, or -1 if the
        /// supplied <paramref name="advisor"/> is not an introduction advisor
        /// for this proxy.
        /// </returns>
        public virtual int IndexOf(IIntroductionAdvisor advisor)
        {
            lock(this.SyncRoot)
            {
                return IndexOfInternal(advisor);
            }
        }

        /// <summary>
        /// Removes the supplied <paramref name="advisor"/> the list of advisors
        /// for this proxy.
        /// </summary>
        /// <param name="advisor">The advisor to remove.</param>
        /// <returns>
        /// <see langword="true"/> if advisor was found in the list of
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/> for this
        /// proxy and was successfully removed; <see langword="false"/> if not
        /// or if the supplied <paramref name="advisor"/> is <cref lang="null"/>.
        /// </returns>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <paramref name="advisor"/> cannot be removed.
        /// </exception>
        public bool RemoveAdvisor(IAdvisor advisor)
        {
            DieIfFrozen("Cannot remove advisor: config is frozen");
            bool wasRemoved = false;
            if (advisor != null)
            {
                lock(this.SyncRoot)
                {
                    int index = IndexOf(advisor);
                    if (index == -1)
                    {
                        wasRemoved = false;
                    }
                    else
                    {
                        RemoveAdvisorInternal(index);
                        wasRemoved = true;
                    }
                }
            }
            return wasRemoved;
        }

        /// <summary>
        /// Removes the <see cref="Spring.Aop.IAdvisor"/> at the supplied
        /// <paramref name="index"/> in the
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/> list
        /// from the list of
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/> for this proxy.
        /// </summary>
        /// <param name="index">
        /// The index of the <see cref="Spring.Aop.IAdvisor"/> to remove.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <see cref="Spring.Aop.IAdvisor"/> at the supplied <paramref name="index"/>
        /// cannot be removed; or if the supplied <paramref name="index"/> is out of
        /// range.
        /// </exception>
        public virtual void RemoveAdvisor(int index)
        {
            DieIfFrozen("Cannot remove advisor: config is frozen");
            lock(this.SyncRoot)
            {
                RemoveAdvisorInternal(index);
            }
        }

        /// <summary>
        /// Removes the supplied <paramref name="advice"/> from the list
        /// of <see cref="Spring.Aop.Framework.IAdvised.Advisors"/>.
        /// </summary>
        /// <param name="advice">
        /// The <see cref="AopAlliance.Aop.IAdvice"/> to remove.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the supplied <paramref name="advice"/> was
        /// found in the list of <see cref="Spring.Aop.Framework.IAdvised.Advisors"/>
        /// and successfully removed.
        /// </returns>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <see cref="AopAlliance.Aop.IAdvice"/> cannot be removed.
        /// </exception>
        public bool RemoveAdvice(IAdvice advice)
        {
            lock(this.SyncRoot)
            {
                int index = IndexOf(advice);
                if (index == -1)
                {
                    return false;
                }
                else
                {
                    RemoveAdvisorInternal(index);
                    return true;
                }
            }
        }

        /// <summary>
        /// Removes the supplied <paramref name="introduction"/> from the list
        /// of <see cref="Spring.Aop.Framework.AdvisedSupport.Introductions"/>.
        /// </summary>
        /// <param name="introduction">
        /// The <see cref="Spring.Aop.IIntroductionAdvisor"/> to remove.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the supplied <paramref name="introduction"/> was
        /// found in the list of <see cref="Spring.Aop.Framework.AdvisedSupport.Introductions"/>
        /// and successfully removed.
        /// </returns>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <see cref="Spring.Aop.IIntroductionAdvisor"/> cannot be removed.
        /// </exception>
        public bool RemoveIntroduction(IIntroductionAdvisor introduction)
        {
            DieIfFrozen("Cannot remove introduction: config is frozen");
            bool wasRemoved = false;
            if (introduction != null)
            {
                lock(this.SyncRoot)
                {
                    int index = IndexOf(introduction);
                    if (index == -1)
                    {
                        wasRemoved = false;
                    }
                    else
                    {
                        RemoveIntroduction(index);
                        wasRemoved = true;
                    }
                }
            }
            return wasRemoved;
        }

        /// <summary>
        /// Removes the <see cref="Spring.Aop.IIntroductionAdvisor"/> at the supplied
        /// <paramref name="index"/> in the list of
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Introductions"/> for this proxy.
        /// </summary>
        /// <param name="index">The index of the advisor to remove.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <see cref="Spring.Aop.IIntroductionAdvisor"/> at the supplied
        /// <paramref name="index"/> cannot be removed; or if the supplied
        /// <paramref name="index"/> is out of range.
        /// </exception>
        public virtual void RemoveIntroduction(int index)
        {
            DieIfFrozen("Cannot remove introduction: config is frozen");
            lock(this.SyncRoot)
            {
                if (index < 0 || index >= _introductions.Count)
                {
                    throw new AopConfigException(
                        "Introduction index " + index + " is out of bounds: Only have " + _introductions.Count +
                        " introductions.");
                }
                IIntroductionAdvisor advisor = (IIntroductionAdvisor) _introductions[index];
                // remove all interfaces introduced by the advisor...
                foreach (Type intf in advisor.Interfaces)
                {
                    RemoveInterface(intf);
                }
                this._introductions.RemoveAt(index);
                UpdateIntroductionsArray();
            }
        }

//
//        /// <summary>
//        /// Removes the supplied <paramref name="interceptor"/> from the list of
//        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/> for this
//        /// proxy.
//        /// </summary>
//        /// <param name="interceptor">
//        /// The <see cref="AopAlliance.Intercept.IInterceptor"/> to be removed.
//        /// </param>
//        /// <exception cref="AopConfigException">
//        /// If this proxy configuration is frozen and the
//        /// <paramref name="interceptor"/> cannot be added.
//        /// </exception>
//        public bool RemoveInterceptor(IInterceptor interceptor)
//        {
//            AssertFrozen("Cannot remove interceptor: config is frozen");
//            int index = IndexOf(interceptor);
//            if (index == -1)
//            {
//                return false;
//            }
//            else
//            {
//                RemoveAdvisor(index);
//                return true;
//            }
//        }

        /// <summary>
        /// Adds the supplied <paramref name="advisor"/> to the list
        /// of <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/>.
        /// </summary>
        /// <param name="index">
        /// The index in the <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/>
        /// list at which the supplied <paramref name="advisor"/>
        /// is to be inserted. If -1, appends to the end of the list.
        /// </param>
        /// <param name="advisor">
        /// The <see cref="Spring.Aop.IIntroductionAdvisor"/> to add.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <paramref name="advisor"/> cannot be added.
        /// </exception>
        public virtual void AddAdvisor(int index, IAdvisor advisor)
        {
            DieIfFrozen("Cannot add advisor: config is frozen");
            lock(this.SyncRoot)
            {
                // advisor already in list (SPRNET-846)
                if (_advisors.Contains(advisor)) return;

                if(index == -1)
                {
                    this._advisors.Add(advisor);
                }
                else
                {
                    this._advisors.Insert(index, advisor);
                }
                UpdateAdvisorsArray();
                InterceptorsChanged();
            }
        }

        /// <summary>
        /// Adds the supplied <paramref name="advisor"/> to the list
        /// of <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/>.
        /// </summary>
        /// <param name="advisor">
        /// The <see cref="Spring.Aop.IAdvisor"/> to add.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <paramref name="advisor"/> cannot be added.
        /// </exception>
        public virtual void AddAdvisor(IAdvisor advisor)
        {
            AddAdvisor(this._advisors.Count, advisor);
        }

        /// <summary>
        /// Adds the advisors from the supplied <paramref name="advisors"/> 
        /// to the list of <see cref="Spring.Aop.Framework.IAdvised.Advisors"/>.
        /// </summary>
        /// <param name="advisors">
        /// The <see cref="IAdvisors"/> to add advisors from.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <paramref name="advisors"/> cannot be added.
        /// </exception>
        public void AddAdvisors(IAdvisors advisors)
        {
            foreach (IAdvisor advisor in advisors.Advisors)
            {
                if (advisor is IIntroductionAdvisor)
                {
                    AddIntroduction((IIntroductionAdvisor) advisor);
                }
                else
                {
                    AddAdvisor(advisor);
                }
            }
        }

        /// <summary>
        /// Adds the supplied <paramref name="introductionAdvisor"/> to the list
        /// of <see cref="Spring.Aop.Framework.AdvisedSupport.Introductions"/>.
        /// </summary>
        /// <param name="index">
        /// The index in the <see cref="Spring.Aop.Framework.AdvisedSupport.Introductions"/>
        /// list at which the supplied <paramref name="introductionAdvisor"/>
        /// is to be inserted.
        /// </param>
        /// <param name="introductionAdvisor">
        /// The <see cref="Spring.Aop.IIntroductionAdvisor"/> to add.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <paramref name="introductionAdvisor"/> cannot be added.
        /// </exception>
        public virtual void AddIntroduction(int index, IIntroductionAdvisor introductionAdvisor)
        {
            DieIfFrozen("Cannot add introduction: config is frozen");
            introductionAdvisor.ValidateInterfaces();

            lock(this.SyncRoot)
            {
                if (index < this._introductions.Count)
                {
                    this._introductions.RemoveAt(index);
                }
                this._introductions.Insert(index, introductionAdvisor);

                int intfCount = this.interfaceMap.Count;
                // If the advisor passed validation we can make the change
                foreach (Type intf in introductionAdvisor.Interfaces)
                {
                    this.interfaceMap[intf] = introductionAdvisor;
                }
                UpdateIntroductionsArray();
                if (this.interfaceMap.Count != intfCount)
                {
                    InterfacesChanged();
                }
            }
        }

        /// <summary>
        /// Adds the supplied <paramref name="introductionAdvisor"/> to the list
        /// of <see cref="Spring.Aop.Framework.AdvisedSupport.Introductions"/>.
        /// </summary>
        /// <param name="introductionAdvisor">
        /// The <see cref="Spring.Aop.IIntroductionAdvisor"/> to add.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <paramref name="introductionAdvisor"/> cannot be added.
        /// </exception>
        public virtual void AddIntroduction(IIntroductionAdvisor introductionAdvisor)
        {
            Type introductionType = introductionAdvisor.Advice.GetType();
            lock(this.SyncRoot)
            {
                int pos = this._introductions.Count;
                for (int i = 0; i < pos; i++)
                {
                    IIntroductionAdvisor introduction
                        = (IIntroductionAdvisor) this._introductions[i];
                    if (introduction.Advice.GetType() == introductionType)
                    {
                        pos = i;
                    }
                }
                AddIntroduction(pos, introductionAdvisor);
            }
        }

        /// <summary>
        /// Replaces the <see cref="Spring.Aop.IIntroductionAdvisor"/> that
        /// exists at the supplied <paramref name="index"/> in the list of
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Introductions"/>
        /// with the supplied <paramref name="introduction"/>.
        /// </summary>
        /// <param name="index">
        /// The index of the <see cref="Spring.Aop.IIntroductionAdvisor"/>
        /// in the list of
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Introductions"/>
        /// that is to be replaced.
        /// </param>
        /// <param name="introduction">
        /// The new (replacement) <see cref="Spring.Aop.IIntroductionAdvisor"/>.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If the supplied <paramref name="index"/> is out of range.
        /// </exception>
        public virtual void ReplaceIntroduction(int index, IIntroductionAdvisor introduction)
        {
            lock(this.SyncRoot)
            {
                if(index < 0 || index >= _introductions.Count)
                {
                    throw new AopConfigException(
                        "Introduction index " + index + " is out of bounds:" +
                        " there are currently " + _introductions.Count +
                        " introductions." );
                }

                _introductions[index] = introduction;
            }
        }

        /// <summary> 
        /// Replaces the <paramref name="oldAdvisor"/> with the
        /// <paramref name="newAdvisor"/>.
        /// </summary>
        /// <param name="oldAdvisor">
        /// The original (old) advisor to be replaced.
        /// </param>
        /// <param name="newAdvisor">
        /// The new advisor to replace the <paramref name="oldAdvisor"/> with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="oldAdvisor"/> was
        /// replaced; if the <paramref name="oldAdvisor"/> was not found in the
        /// advisors collection (or the <paramref name="newAdvisor"/> is
        /// <see lang="null"/>, this method returns <see langword="false"/>
        /// and (effectively) does nothing.
        /// </returns>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <paramref name="oldAdvisor"/> cannot be replaced.
        /// </exception>
        /// <seealso cref="Spring.Aop.Framework.ProxyConfig.IsFrozen"/>
        public bool ReplaceAdvisor(IAdvisor oldAdvisor, IAdvisor newAdvisor)
        {
            DieIfFrozen("Cannot replace advisor: config is frozen.");
            lock(this.SyncRoot)
            {
                int index = IndexOf(oldAdvisor);
                if (index == -1 || newAdvisor == null)
                {
                    return false;
                }
                RemoveAdvisor(index);
                AddAdvisor(index, newAdvisor);
            }
            return true;
        }

        /// <summary>
        /// As <see cref="System.Object.ToString()"/> will normally be passed straight through
        /// to the advised target, this method returns the <see cref="System.Object.ToString()"/>
        /// equivalent for the AOP proxy itself.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> description of the proxy configuration.
        /// </returns>
        public virtual string ToProxyConfigString()
        {
            lock(this.SyncRoot)
            {
                return ToStringInternal();
            }
        }

        #endregion

        #region ITargetTypeAware implementation

        /// <summary>
        /// Gets the target type behind the implementing object.
        /// Ttypically a proxy configuration or an actual proxy.
        /// </summary>
        /// <value>The type of the target or null if not known.</value>
        public Type TargetType
        {
            get { return TargetSource.TargetType; }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Sets the target object that is to be advised.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This is a convenience write-only property that allows client code
        /// to set the target object... the target object will be implicitly
        /// wrapped within a new
        /// <see cref="Spring.Aop.Target.SingletonTargetSource"/> instance.
        /// </p>
        /// </remarks>
        public virtual object Target
        {
            set { TargetSource = new SingletonTargetSource(value); }
        }

        /// <summary>
        /// Called by subclasses to get a value indicating whether any AOP proxies have been created yet.
        /// </summary>
        /// <value><c>true</c> if this AOp proxies have been created; otherwise, <c>false</c>.</value>
        protected bool IsActive
        {
            get { return isActive; }
        }

        #endregion

        /// <summary>
        /// Specifies the <see cref="System.Type"/> of proxies that are to be
        /// created for this instance of proxy config.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If this property value is <cref lang="null"/> it simply means that
        /// no proxies have been created yet. Only when the first proxy is
        /// created will this property value be set by the AOP framework.
        /// </p>
        /// <p>
        /// Users will be able to add interceptors dynamically without proxy
        /// regeneration, but if they add introductions the proxy
        /// <see cref="System.Type"/> will have to be regenerated.
        /// </p>
        /// </remarks>
        /// <value>
        /// The <see cref="System.Type"/> of proxies that are to be
        /// created for this instance of proxy config; <cref lang="null"/> if
        /// no proxies have been created yet.
        /// </value>
        internal Type ProxyType
        {
            get { return this.proxyType; }
            set { this.proxyType = value; }
        }

        /// <summary>
        /// Caches proxy constructor for performance reasons.
        /// </summary>
        internal ConstructorInfo ProxyConstructor
        {
            get { return this.proxyConstructor; }
            set { this.proxyConstructor = value; }
        }

        /// <summary>
        /// Registers the supplied <paramref name="listener"/> as a listener for
        /// <see cref="Spring.Aop.Framework.AdvisedSupport"/> notifications.
        /// </summary>
        /// <param name="listener">
        /// The <see cref="Spring.Aop.Framework.IAdvisedSupportListener"/> to
        /// register.
        /// </param>
        public virtual void AddListener(IAdvisedSupportListener listener)
        {
            lock(this.SyncRoot)
            {
                this.listeners.Add(listener);
            }
        }

        /// <summary>
        /// Removes the supplied <paramref name="listener"/>.
        /// </summary>
        /// <param name="listener">
        /// The <see cref="Spring.Aop.Framework.IAdvisedSupportListener"/> to
        /// be removed.
        /// </param>
        public virtual void RemoveListener(IAdvisedSupportListener listener)
        {
            lock(this.SyncRoot)
            {
                this.listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Adds a new interface to the list of interfaces that are proxied by this proxy.
        /// </summary>
        /// <param name="intf">
        /// The interface to be proxied by this proxy.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen
        /// (<see cref="Spring.Aop.Framework.ProxyConfig.IsFrozen"/>);
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="intf"/> is <see langword="null"/>.
        /// </exception>
        public virtual void AddInterface(Type intf)
        {
        	DieIfFrozen("Cannot add interface: configuration is frozen.");
            AssertUtils.ArgumentNotNull(intf, "intf", "Cannot proxy a null interface.");

            lock(this.SyncRoot)
            {
                AddInterfaceInternal(intf);
                InterfacesChanged();
            }
        }

        /// <summary>
        /// Adds a new interface to the list of interfaces that are proxied by this proxy.
        /// </summary>
        /// <param name="intf">
        /// The interface to be proxied by this proxy.
        /// </param>
        /// <remarks>
        /// Access is not synchronized.
        /// </remarks>
        protected virtual void AddInterfaceInternal(Type intf)
        {
            this.interfaceMap[intf] = null;
        }

        /// <summary>
        /// Removes the supplied (proxied) <paramref name="intf"/>.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Does nothing if the supplied (proxied) <paramref name="intf"/>
        /// isn't proxied.
        /// </p>
        /// </remarks>
        /// <param name="intf">The interface to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the interface was removed.</returns>
        public virtual bool RemoveInterface(Type intf)
        {
        	DieIfFrozen("Cannot remove interface: configuration is frozen.");
            lock(this.SyncRoot)
            {
                if (intf != null && this.interfaceMap.Contains(intf))
                {
                    this.interfaceMap.Remove(intf);
                    InterfacesChanged();
                    return true;
                }
            }
            return false;
        }

        /// <summary> 
        /// Return the index (0 based) of the supplied
        /// <see cref="AopAlliance.Aop.IAdvice"/> in the interceptor
        /// (advice) chain for this proxy.
        /// </summary>
        /// <remarks>
        /// <p>
        /// The return value of this method can be used to index into
        /// the <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/>
        /// list.
        /// </p>
        /// </remarks>
        /// <param name="advice">
        /// The <see cref="AopAlliance.Aop.IAdvice"/> to search for.
        /// </param>
        /// <returns>
        /// The zero (0) based index of this interceptor, or -1 if the
        /// supplied <paramref name="advice"/> is not an advice for this
        /// proxy.
        /// </returns>
        public virtual int IndexOf(IAdvice advice)
        {
            lock(this.SyncRoot)
            {
                return IndexOfInternal(advice);
            }
        }

        /// <summary> 
        /// Return the index (0 based) of the supplied
        /// <see cref="AopAlliance.Aop.IAdvice"/> in the interceptor
        /// (advice) chain for this proxy.
        /// </summary>
        /// <remarks>
        /// <p>Acces is not synchronized</p>
        /// <p>
        /// The return value of this method can be used to index into
        /// the <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/>
        /// list.
        /// </p>
        /// </remarks>
        /// <param name="advice">
        /// The <see cref="AopAlliance.Aop.IAdvice"/> to search for.
        /// </param>
        /// <returns>
        /// The zero (0) based index of this interceptor, or -1 if the
        /// supplied <paramref name="advice"/> is not an advice for this
        /// proxy.
        /// </returns>
        private int IndexOfInternal(IAdvice advice)
        {
            if (this._advisors != null)
            {
                for (int i = 0; i < this._advisors.Count; ++i)
                {
                    IAdvisor advisor = (IAdvisor) this._advisors[i];
                    if (advisor.Advice == advice)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary> 
        /// Return the index (0 based) of the supplied
        /// <see cref="Spring.Aop.IAdvisor"/> in the interceptor
        /// (advice) chain for this proxy.
        /// </summary>
        /// <param name="advisor">
        /// The <see cref="Spring.Aop.IAdvisor"/> to search for.
        /// </param>
        /// <returns>
        /// The zero (0) based index of this advisor, or -1 if the
        /// supplied <paramref name="advisor"/> is not an advisor for this
        /// proxy.
        /// </returns>
        /// <remarks>
        /// Access is not synchronized.
        /// </remarks>
        private int IndexOfInternal(IAdvisor advisor)
        {
            return this._advisors != null ? this._advisors.IndexOf(advisor) : -1;
        }

        /// <summary> 
        /// Return the index (0 based) of the supplied
        /// <see cref="Spring.Aop.IIntroductionAdvisor"/> in the introductions
        /// for this proxy.
        /// </summary>
        /// <param name="advisor">
        /// The <see cref="Spring.Aop.IIntroductionAdvisor"/> to search for.
        /// </param>
        /// <returns>
        /// The zero (0) based index of this advisor, or -1 if the
        /// supplied <paramref name="advisor"/> is not an introduction advisor
        /// for this proxy.
        /// </returns>
        /// <remarks>
        /// Access is not synchronized
        /// </remarks>
        private int IndexOfInternal(IIntroductionAdvisor advisor)
        {
            return this._introductions.IndexOf(advisor);
        }

        /// <summary>
        /// Removes the <see cref="Spring.Aop.IAdvisor"/> at the supplied
        /// <paramref name="index"/> in the
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/> list
        /// from the list of
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.Advisors"/> for this proxy.
        /// </summary>
        /// <param name="index">
        /// The index of the <see cref="Spring.Aop.IAdvisor"/> to remove.
        /// </param>
        /// <exception cref="AopConfigException">
        /// If this proxy configuration is frozen and the
        /// <see cref="Spring.Aop.IAdvisor"/> at the supplied <paramref name="index"/>
        /// cannot be removed; or if the supplied <paramref name="index"/> is out of
        /// range.
        /// </exception>
        /// <remarks>
        /// Does not synchronize access.
        /// </remarks>
        private void RemoveAdvisorInternal(int index)
        {
            if (index < 0 || index >= this._advisors.Count)
            {
                throw
                    new AopConfigException(
                        "Advisor index " + index + " is out of bounds: Only have " + this._advisors.Count + " advisors");
            }
            this._advisors.RemoveAt(index);
            this.UpdateAdvisorsArray();
            this.InterceptorsChanged();
        }

        /// <summary>
        /// Is the supplied <paramref name="advice"/> included in any
        /// advisor?
        /// </summary>
        /// <param name="advice">
        /// The <see cref="AopAlliance.Aop.IAdvice"/> to check for the
        /// inclusion of.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the supplied <paramref name="advice"/>
        /// could be run in an invocation (this does not imply that said
        /// <paramref name="advice"/> will be run).
        /// </returns>
        public bool AdviceIncluded(IAdvice advice)
        {
            return (IndexOf(advice) != -1);
        }

        /// <summary>
        /// Returns a count of all of the <see cref="AopAlliance.Aop.IAdvice"/>
        /// type-compatible with the supplied <see cref="System.Type"/>.
        /// </summary>
        /// <param name="interceptorType">
        /// The <see cref="System.Type"/> of the
        /// <see cref="AopAlliance.Aop.IAdvice"/> to check.
        /// </param>
        /// <returns>
        /// A count of all of the <see cref="AopAlliance.Aop.IAdvice"/>
        /// type-compatible with the supplied <see cref="System.Type"/>.
        /// </returns>
        public int CountAdviceOfType(Type interceptorType)
        {
            int count = 0;
            lock(this.SyncRoot)
            {
                foreach (IAdvisor advisor in this._advisors)
                {
                    if (interceptorType.IsAssignableFrom(advisor.Advice.GetType()))
                    {
                        ++count;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Throws an <see cref="Spring.Aop.Framework.AopConfigException"/> if
        /// this instances proxy configuration data is frozen.
        /// </summary>
        /// <param name="message">
        /// The message that will be passed through to the constructor of any
        /// thrown <see cref="Spring.Aop.Framework.AopConfigException"/>.
        /// </param>
        /// <exception cref="Spring.Aop.Framework.AopConfigException">
        /// If the configuration for this proxy is frozen.
        /// </exception>
        /// <seealso cref="Spring.Aop.Framework.IAdvised.IsFrozen"/>
        private void DieIfFrozen(string message)
        {
            if (IsFrozen)
            {
                throw new AopConfigException(message);
            }
        }

        /// <summary>
        /// Bring the advisors array up to date with the list.
        /// </summary>
        private void UpdateAdvisorsArray()
        {
            this._advisorsArray = new IAdvisor[this._advisors.Count];
            this._advisors.CopyTo(this._advisorsArray, 0);
        }

        /// <summary>
        /// Bring the introductions array up to date with the list.
        /// </summary>
        private void UpdateIntroductionsArray()
        {
            this._introductionsArray = new IIntroductionAdvisor[this._introductions.Count];
            this._introductions.CopyTo(this._introductionsArray, 0);
        }

        /// <summary>
        /// Callback method that is invoked when the list of proxied interfaces
        /// has changed.
        /// </summary>
        /// <remarks>
        /// <p>
        /// An example of such a change would be when a new introduction is
        /// added. Resetting
        /// <see cref="Spring.Aop.Framework.AdvisedSupport.ProxyType"/> to
        /// <cref lang="null"/> will cause a new proxy <see cref="System.Type"/>
        /// to be generated on the next call to get a proxy.
        /// </p>
        /// </remarks>
        private void InterfacesChanged()
        {
            ProxyType = null;
            if (this.isActive)
            {
                foreach (IAdvisedSupportListener listener in this.listeners)
                {
                    listener.InterfacesChanged(this);
                }
            }
        }

        /// <summary>
        /// Callback method that is invoked when the interceptor list has changed.
        /// </summary>
        private void InterceptorsChanged()
        {
            if (this.isActive)
            {
                foreach (IAdvisedSupportListener listener in this.listeners)
                {
                    listener.AdviceChanged(this);
                }
            }
        }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        protected void Activate()
        {
            lock (this.SyncRoot)
            {
                this.isActive = true;
                foreach (IAdvisedSupportListener listener in this.listeners)
                {
                    listener.Activated(this);
                }
            }
        }

        /// <summary> 
        /// Creates an AOP proxy using this instance's configuration data.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Subclasses must not create a proxy by any other means (at least
        /// without having a well thought out and cogent reason for doing so).
        /// This is because the implementation of this method performs some
        /// required housekeeping logic prior to creating an AOP proxy.
        /// </p>
        /// </remarks>
        /// <seealso cref="Spring.Aop.Framework.IAopProxyFactory.CreateAopProxy"/>
        protected internal virtual IAopProxy CreateAopProxy()
        {
            lock (this.SyncRoot)
            {
                if (!this.isActive)
                {
                    Activate();
                }
                return AopProxyFactory.CreateAopProxy(this);
            }
        }

        /// <summary>
        /// Copies the configuration from the supplied other
        /// <see cref="Spring.Aop.Framework.AdvisedSupport"/> into this instance.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Useful when this instance has been created using the no-argument
        /// constructor, and needs to get all of its confiuration data from
        /// another <see cref="Spring.Aop.Framework.AdvisedSupport"/> (most
        /// usually to have an independant copy of said configuration data).
        /// </p>
        /// </remarks>
        /// <param name="other">
        /// The <see cref="Spring.Aop.Framework.AdvisedSupport"/> instance
        /// containing the configiration data that is to be copied into this
        /// instance.
        /// </param>
        protected internal virtual void CopyConfigurationFrom(AdvisedSupport other)
        {
            CopyFrom(other);
            this.m_targetSource = other.m_targetSource;
            this.proxyType = other.proxyType;
            this.proxyConstructor = other.proxyConstructor;

            foreach (Type intf in other.interfaceMap.Keys)
            {
                this.interfaceMap[intf] = other.interfaceMap[intf];
            }
            this._advisors = new ArrayList();
            foreach (IAdvisor advisor in other._advisors)
            {
                AddAdvisor(advisor);
            }
            this._introductions = new ArrayList();
            foreach (IIntroductionAdvisor advisor in other._introductions)
            {
                AddIntroduction(advisor);
            }
        }

        /// <summary>
        /// A <see cref="System.String"/> that represents the current
        /// <see cref="Spring.Aop.Framework.ProxyConfig"/> configuration.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current
        /// <see cref="Spring.Aop.Framework.ProxyConfig"/> configuration.
        /// </returns>
        public override string ToString()
        {
            lock(this.SyncRoot)
            {
                return ToStringInternal();
            }
        }

        /// <summary>
        /// A <see cref="System.String"/> that represents the current
        /// <see cref="Spring.Aop.Framework.ProxyConfig"/> configuration.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current
        /// <see cref="Spring.Aop.Framework.ProxyConfig"/> configuration.
        /// </returns>
        private string ToStringInternal()
        {
            StringBuilder buffer = new StringBuilder(this.GetType().FullName + ":\n");
            buffer.Append(this.interfaceMap.Count + " interfaces=[");
            this.InterfacesToString(buffer);
            buffer.Append("];\n");
            buffer.Append(this._advisors.Count + " pointcuts=[");
            this.AdvisorsToString(buffer);
            buffer.Append("];\n");
            buffer.Append("targetSource=[" + this.m_targetSource + "];\n");
            buffer.Append("advisorChainFactory=" + this.advisorChainFactory + ";\n");
            buffer.Append(base.ToString());
            return buffer.ToString();
        }

        /// <summary>
        /// Helper method that adds the names of all of the proxied interfaces
        /// to the buffer of the supplied <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="System.Text.StringBuilder"/> to append the proxied interface
        /// names to.
        /// </param>
        private void InterfacesToString(StringBuilder buffer)
        {
            string separator = string.Empty;
            foreach (Type intf in this.interfaceMap.Keys)
            {
                buffer.Append(separator).Append("[").Append(intf.FullName).Append("] -> ");
                IIntroductionAdvisor advisor = this.interfaceMap[intf] as IIntroductionAdvisor;
                if (advisor == null)
                {
                    if (TargetSource.TargetType != null)
                    {
                        buffer.Append("target[").Append(TargetSource.TargetType.FullName).Append("]");
                    }
                    else
                    {
                        buffer.Append("target[NOT SPECIFIED (=null)]");
                    }
                }
                else
                {
                    buffer.Append("introduction[").Append(advisor.Advice.GetType().FullName).Append("]");
                }
                separator = ", ";
            }
        }

        /// <summary>
        /// Helper method that adds advisor's <see cref="System.Object.ToString"/>
        /// to the buffer of the supplied <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="System.Text.StringBuilder"/> to append the advisor details to.
        /// </param>
        private void AdvisorsToString(StringBuilder buffer)
        {
            string separator = string.Empty;
            foreach (IAdvisor advisor in this._advisors)
            {
                buffer.Append(separator).Append(advisor);
                separator = ", ";
            }
        }

        /// <summary>
        /// Gets all of the interfaces implemented by the
        /// <see cref="System.Type"/> of the supplied
        /// <paramref name="target"/>.
        /// </summary>
        /// <param name="target">
        /// The object to get the interfaces of.
        /// </param>
        /// <returns>
        /// All of the interfaces implemented by the
        /// <see cref="System.Type"/> of the supplied
        /// <paramref name="target"/>.
        /// </returns>
        /// <exception cref="AopConfigException">
        /// If the supplied <paramref name="target"/> is <see langword="null"/>.
        /// </exception>
        protected static Type[] GetInterfaces(object target)
        {
            if (target == null)
            {
                throw new AopConfigException("Can't proxy null object");
            }
            return ReflectionUtils.GetInterfaces(target.GetType());
        }
    }
}