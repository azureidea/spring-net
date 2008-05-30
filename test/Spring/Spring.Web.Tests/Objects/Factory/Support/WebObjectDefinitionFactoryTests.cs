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
using NUnitAspEx;
using Spring.TestSupport;

#endregion

namespace Spring.Objects.Factory.Support
{
    /// <summary>
    /// Unit tests for the WebObjectDefinitionFactory class.
    /// </summary>
    /// <author>Erich Eichinger</author>
    /// <version>$Id: WebObjectDefinitionFactoryTests.cs,v 1.4 2008/03/14 12:02:45 oakinger Exp $</version>
    [AspTestFixture("/Test", "/Spring/Objects/Factory/Support")]
    public class WebObjectDefinitionFactoryTests
    {
        [Test]
        public void CreateRootDefinition()
        {
            WebObjectDefinitionFactory factory = new WebObjectDefinitionFactory();
            IConfigurableObjectDefinition definition
                = factory.CreateObjectDefinition(
                    typeof(TestObject).FullName, null, AppDomain.CurrentDomain);
            Assert.IsNotNull(definition, "CreateObjectDefinition with no parent is returning null (it must never do so).");
            Assert.AreEqual(typeof(TestObject), definition.ObjectType);
            Assert.AreEqual(0, definition.PropertyValues.PropertyValues.Length,
                            "Must not have any property values as none were passed in.");
            Assert.AreEqual(0, definition.ConstructorArgumentValues.ArgumentCount,
                            "Must not have any ctor args as none were passed in.");
        }

        [Test]
        public void CreateChildDefinition()
        {
            WebObjectDefinitionFactory factory = new WebObjectDefinitionFactory();
            IConfigurableObjectDefinition definition
                = factory.CreateObjectDefinition(
                    typeof(TestObject).FullName, "Aimee Mann", AppDomain.CurrentDomain);
            Assert.IsNotNull(definition, "CreateObjectDefinition with no parent is returning null (it must never do so).");
            Assert.AreEqual(typeof(TestObject), definition.ObjectType);
            Assert.AreEqual(0, definition.PropertyValues.PropertyValues.Length,
                            "Must not have any property values as none were passed in.");
            Assert.AreEqual(0, definition.ConstructorArgumentValues.ArgumentCount,
                            "Must not have any ctor args as none were passed in.");
        }

        [Test]
        public void DoesNotResolveTypeNameToFullTypeInstanceIfAppDomainIsNull()
        {
            WebObjectDefinitionFactory factory = new WebObjectDefinitionFactory();
            IConfigurableObjectDefinition definition
                = factory.CreateObjectDefinition(
                    typeof(TestObject).FullName, null, null);
            Assert.IsNotNull(definition, "CreateObjectDefinition with no parent is returning null (it must never do so).");
            Assert.AreEqual(typeof(TestObject).FullName, definition.ObjectTypeName);
            Assert.AreEqual(0, definition.PropertyValues.PropertyValues.Length,
                            "Must not have any property values as none were passed in.");
            Assert.AreEqual(0, definition.ConstructorArgumentValues.ArgumentCount,
                            "Must not have any ctor args as none were passed in.");
        }

        [Test]
        public void ResolvesToPageRootDefinitionIfEndsWithASPX()
        {
            using (TestWebContext ctx = new TestWebContext("/Test", "testform.aspx"))
            {
                WebObjectDefinitionFactory factory = new WebObjectDefinitionFactory();
                IWebObjectDefinition definition
                    = (IWebObjectDefinition)factory.CreateObjectDefinition("/Test/testform.aspx", null, AppDomain.CurrentDomain);
                Assert.IsNotNull(definition, "CreateObjectDefinition with no parent is returning null (it must never do so).");
                Assert.IsTrue(definition.IsPage, ".aspx extension must result in a page instance");
                Assert.AreEqual(typeof(RootWebObjectDefinition), definition.GetType());
            }
        }

        [Test]
        public void ResolvesToPageChildDefinitionIfEndsWithASPX()
        {
            using (TestWebContext ctx = new TestWebContext("/Test", "testform.aspx"))
            {
                WebObjectDefinitionFactory factory = new WebObjectDefinitionFactory();
                IWebObjectDefinition definition
                    = (IWebObjectDefinition)factory.CreateObjectDefinition("/Test/testform.aspx", "parentdefinition", AppDomain.CurrentDomain);
                Assert.IsNotNull(definition, "CreateObjectDefinition with parent is returning null (it must never do so).");
                Assert.IsTrue(definition.IsPage, ".aspx extension must result in a page instance");
                Assert.AreEqual(typeof(ChildWebObjectDefinition), definition.GetType());
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectCreationException))]
        public void ThrowsArgumentExceptionOnNonExistingPath()
        {
            using (TestWebContext ctx = new TestWebContext("/Test", "testform.aspx"))
            {
                WebObjectDefinitionFactory factory = new WebObjectDefinitionFactory();
                IWebObjectDefinition definition
                    = (IWebObjectDefinition)factory.CreateObjectDefinition("/Test/DoesNotExist.aspx", "parentdefinition", AppDomain.CurrentDomain);
            }
        }
    }
}