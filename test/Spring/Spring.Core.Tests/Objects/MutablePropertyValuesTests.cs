#region License

/*
 * Copyright 2004 the original author or authors.
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

using NUnit.Framework;

#endregion

namespace Spring.Objects
{
	/// <summary>
	/// Unit tests for the MutablePropertyValues class.
    /// </summary>
    /// <author>Rick Evans</author>
    /// <version>$Id: MutablePropertyValuesTests.cs,v 1.2 2006/04/09 07:24:50 markpollack Exp $</version>
	[TestFixture]
    public sealed class MutablePropertyValuesTests
    {
        #region SetUp
        /// <summary>
        /// The setup logic executed before the execution of this test fixture.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetUp () {}

        /// <summary>
        /// The setup logic executed before the execution of each individual test.
        /// </summary>
        [SetUp]
        public void SetUp () {}
        #endregion

        #region TearDown
        /// <summary>
        /// The tear down logic executed after the execution of each individual test.
        /// </summary>
        [TearDown]
        public void TearDown () {}

        /// <summary>
        /// The tear down logic executed after the entire test fixture has executed.
        /// </summary>
        [TestFixtureTearDown]
        public void FixtureTearDown () {}
        #endregion

        [Test]
        public void Instantiation () {
            MutablePropertyValues root = new MutablePropertyValues ();
            root.Add (new PropertyValue ("Name", "Fiona Apple"));
            root.Add (new PropertyValue ("Age", 24));
            MutablePropertyValues props = new MutablePropertyValues (root);
            Assert.AreEqual (2, props.PropertyValues.Length);
        }

        [Test]
        public void InstantiationWithNulls () 
        {
            MutablePropertyValues props = new MutablePropertyValues ((IDictionary) null);
            Assert.AreEqual (0, props.PropertyValues.Length);
            MutablePropertyValues props2 = new MutablePropertyValues ((IPropertyValues) null);
            Assert.AreEqual (0, props2.PropertyValues.Length);
        }

        [Test]
        public void AddAllInList () 
        {
            MutablePropertyValues props = new MutablePropertyValues ();
            props.AddAll (new PropertyValue [] {
                new PropertyValue ("Name", "Fiona Apple"),
                new PropertyValue ("Age", 24)});
            Assert.AreEqual (2, props.PropertyValues.Length);
        }

        [Test]
        public void AddAllInNullList () 
        {
            MutablePropertyValues props = new MutablePropertyValues ();
            props.Add (new PropertyValue ("Name", "Fiona Apple"));
            props.Add (new PropertyValue ("Age", 24));
            props.AddAll ((IList) null);
            Assert.AreEqual (2, props.PropertyValues.Length);
        }

        [Test]
        public void RemoveByName () 
        {
            MutablePropertyValues props = new MutablePropertyValues ();
            props.Add (new PropertyValue ("Name", "Fiona Apple"));
            props.Add (new PropertyValue ("Age", 24));
            Assert.AreEqual (2, props.PropertyValues.Length);
            props.Remove ("name");
            Assert.AreEqual (1, props.PropertyValues.Length);
        }

        [Test]
        public void RemoveByPropertyValue () 
        {
            MutablePropertyValues props = new MutablePropertyValues ();
            PropertyValue propName = new PropertyValue ("Name", "Fiona Apple");
            props.Add (propName);
            props.Add (new PropertyValue ("Age", 24));
            Assert.AreEqual (2, props.PropertyValues.Length);
            props.Remove (propName);
            Assert.AreEqual (1, props.PropertyValues.Length);
        }

        [Test]
        public void Contains () 
        {
            MutablePropertyValues props = new MutablePropertyValues ();
            props.Add (new PropertyValue ("Name", "Fiona Apple"));
            props.Add (new PropertyValue ("Age", 24));
            // must be case insensitive to be CLS compliant...
            Assert.IsTrue (props.Contains ("nAmE"));
        }

        [Test]
        public void AddAllInMap () 
        {
            MutablePropertyValues props = new MutablePropertyValues ();
            IDictionary map = new Hashtable ();
            map.Add ("Name", "Fiona Apple");
            map.Add ("Age", 24);
            props.AddAll (map);
            Assert.AreEqual (2, props.PropertyValues.Length);
        }

        [Test]
        public void AddAllInNullMap () 
        {
            MutablePropertyValues props = new MutablePropertyValues ();
            props.Add (new PropertyValue ("Name", "Fiona Apple"));
            props.AddAll ((IDictionary) null);
            Assert.AreEqual (1, props.PropertyValues.Length);
        }

        [Test]
        public void ChangesSince () 
        {
            IDictionary map = new Hashtable ();
            PropertyValue propName = new PropertyValue ("Name", "Fiona Apple");
            map.Add (propName.Name, propName.Value);
            map.Add ("Age", 24);
            MutablePropertyValues props = new MutablePropertyValues (map);
            MutablePropertyValues newProps = new MutablePropertyValues (map);

            // change the name... this is the change we'll be looking for
            newProps.SetPropertyValueAt (new PropertyValue (propName.Name, "Naomi Woolf"), 0);
            IPropertyValues changes = newProps.ChangesSince (props);
            Assert.AreEqual (1, changes.PropertyValues.Length);
            // the name was changed, so its the name property that should be in the changed list
            Assert.IsTrue (changes.Contains ("name"));

            newProps.Add (new PropertyValue ("Commentator", "Naomi Woolf"));
            changes = newProps.ChangesSince (props);
            Assert.AreEqual (2, changes.PropertyValues.Length);
            // the Commentator was added, so its the Commentator property that should be in the changed list
            Assert.IsTrue (changes.Contains ("commentator"));
            // the name was changed, so its the name property that should be in the changed list
            Assert.IsTrue (changes.Contains ("name"));
        }

        [Test]
        public void ChangesSinceWithSelf () 
        {
            IDictionary map = new Hashtable ();
            map.Add ("Name", "Fiona Apple");
            map.Add ("Age", 24);
            MutablePropertyValues props = new MutablePropertyValues (map);
            props.Remove ("name");
            // get all of the changes between self and self again (there should be none);
            IPropertyValues changes = props.ChangesSince (props);
            Assert.AreEqual (0, changes.PropertyValues.Length);
        }
	}
}
