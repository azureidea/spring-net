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

using NUnit.Framework;

using Spring.Objects;
using Spring.Aop.Interceptor;
using Spring.Proxy;
using System.Reflection;
using Spring.Expressions;

#endregion

namespace Spring.Aop.Framework.DynamicProxy
{
    /// <summary>
    /// Additional and overridden tests for the inheritance-based proxy.
    /// </summary>
    /// <author>Bruno Baia</author>
    /// <version>$Id: InheritanceAopProxyTests.cs,v 1.2 2008/03/03 09:29:17 bbaia Exp $</version>
    [TestFixture]
    public class InheritanceAopProxyTests
    {
        protected object CreateProxy(AdvisedSupport advisedSupport)
        {
            Type proxyType = new InheritanceAopProxyTypeBuilder(advisedSupport).BuildProxyType();
            ConstructorInfo proxyCtorInfo = proxyType.GetConstructor(new Type[] { typeof(IAdvised) });

            ExpressionEvaluator.GetValue(advisedSupport, "Activate()");
            return ((IAopProxy)proxyCtorInfo.Invoke(new object[] { advisedSupport })).GetProxy();
        }

        [Test]
        public void DirectCall()
        {
            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new InheritanceTestObject();

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is InheritanceTestObject);
            InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
            proxiedClass.Name = "DirectCall";
            Assert.AreEqual("DirectCall", proxiedClass.Name);

            Assert.IsTrue(proxy is IIncrementable);
            IIncrementable proxiedIntf = proxy as IIncrementable;
            proxiedIntf.Increment();
            Assert.AreEqual(1, proxiedIntf.Value);            
        }

        [Test]
        public void ProxyTargetTypeOnly()
        {
            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new InheritanceTestObject("ProxyTargetTypeOnly");

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is InheritanceTestObject);
            InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
            Assert.AreNotEqual("ProxyTargetTypeOnly", proxiedClass.Name);
        }

        [Test]
        public void CallBaseConstructor()
        {
            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new InheritanceTestObject();

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is InheritanceTestObject);
            InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
            Assert.AreEqual("InheritanceTestObject", proxiedClass.Name);
        }

        [Test]
        public void InterceptVirtualMethod()
        {
            NopInterceptor ni = new NopInterceptor();

            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new InheritanceTestObject();
            advised.AddAdvice(ni);

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is InheritanceTestObject);
            InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
            proxiedClass.Name = "InterceptVirtualMethod";
            Assert.AreEqual("InterceptVirtualMethod", proxiedClass.Name);
            Assert.AreEqual(2, ni.Count);
        }

#if NET_2_0
        [Test]
        public void InterceptVirtualGenericMethod()
        {
            NopInterceptor ni = new NopInterceptor();

            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new AnotherTestObject();
            advised.AddAdvice(ni);

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is AnotherTestObject);
            AnotherTestObject proxiedClass = proxy as AnotherTestObject;
            Assert.AreEqual(typeof(int), proxiedClass.GenericMethod<int>());
            Assert.AreEqual(1, ni.Count);
        }

        public class AnotherTestObject
        {
            public virtual Type GenericMethod<T>()
            {
                return typeof(T);
            }

            // Test ambiguous match
            public virtual Type GenericMethod()
            {
                return typeof(string);
            }
        }
#endif

        [Test]
        public void DoesNotInterceptFinalMethod()
        {
            NopInterceptor ni = new NopInterceptor();

            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new InheritanceTestObject();
            advised.AddAdvice(ni);

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is InheritanceTestObject);
            InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
            proxiedClass.Name = "DoesNotInterceptFinalMethod";
            Assert.AreEqual("DoesNotInterceptFinalMethod", proxiedClass.Name);
            Assert.AreEqual(2, ni.Count);

            proxiedClass.Reset();
            Assert.AreEqual(2, ni.Count);
            Assert.AreEqual("InheritanceTestObject", proxiedClass.Name);
            Assert.AreEqual(3, ni.Count);
        }

        [Test]
        public void InterceptVirtualMethodThatBelongsToAnInterface()
        {
            NopInterceptor ni = new NopInterceptor();

            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new InheritanceTestObject();
            advised.AddAdvice(ni);

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is InheritanceTestObject);
            InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
            proxiedClass.Increment();
            Assert.AreEqual(1, ni.Count);

            Assert.IsTrue(proxy is IIncrementable);
            IIncrementable proxiedInterface = proxy as IIncrementable;
            proxiedInterface.Increment();
            Assert.AreEqual(2, ni.Count);
        }

        [Test]
        public void InterceptNonVirtualMethodThatBelongsToAnInterface()
        {
            NopInterceptor ni = new NopInterceptor();

            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new InheritanceTestObject();
            advised.AddAdvice(ni);

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is InheritanceTestObject);
            InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
            Assert.AreEqual(0, proxiedClass.Value);
            Assert.AreEqual(0, ni.Count);

            Assert.IsTrue(proxy is IIncrementable);
            IIncrementable proxiedInterface = proxy as IIncrementable;
            proxiedInterface.Increment();
            Assert.AreEqual(1, proxiedInterface.Value);
            Assert.AreEqual(2, ni.Count);
        }

        [Test]
        public void InterceptThisCalls()
        {
            NopInterceptor ni = new NopInterceptor();

            AdvisedSupport advised = new AdvisedSupport();
            advised.Target = new InheritanceTestObject();
            advised.AddAdvice(ni);

            object proxy = CreateProxy(advised);
            //DynamicProxyManager.SaveAssembly();

            Assert.IsTrue(proxy is InheritanceTestObject);
            InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
            proxiedClass.IncrementTwice();
            Assert.AreEqual(2, ni.Count);
            Assert.AreEqual(2, proxiedClass.Value);
        }

        //[Test]
        //public void InterceptProtectedMethod()
        //{
        //    NopInterceptor ni = new NopInterceptor();

        //    AdvisedSupport advised = new AdvisedSupport();
        //    advised.Target = new InheritanceTestObject();
        //    advised.AddAdvice(ni);

        //    object proxy = CreateProxy(advised);
        //    //DynamicProxyManager.SaveAssembly();

        //    Assert.IsTrue(proxy is InheritanceTestObject);
        //    InheritanceTestObject proxiedClass = proxy as InheritanceTestObject;
        //    proxiedClass.Todo();
        //    Assert.AreEqual(1, ni.Count);
        //}
    }

    #region Helper Classes

    public interface IIncrementable
    {
        int Value {get; set;}
        void Increment();
    }

    public class InheritanceTestObject : IIncrementable
    {
        // virtual method
        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        // many constructors
        public InheritanceTestObject() : this("InheritanceTestObject", 0)
        {
        }

        public InheritanceTestObject(string name) : this(name, 0)
        {
        }

        public InheritanceTestObject(string name, int value)
        {
            this._name = name;
            this._value = value;
        }

        #region IIncrementable Members

        // non virtual method that belongs to an interface
        private int _value;
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        // virtual method that belongs to an interface too
        public virtual void Increment()
        {
            // this call
            this._value++;
        }

        #endregion

        // final method
        public void Reset()
        {
            this._name = "InheritanceTestObject";
            this._value = 0;
        }

        // this call
        public void IncrementTwice()
        {
            this.Increment();
            this.Increment();
        }

        // protected method call
        public void Todo()
        {
            ProtectedTodo();
        }

        protected virtual void ProtectedTodo()
        {

        }
    }

    #endregion
}