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

using NUnit.Framework;

using Spring.Expressions;

#endregion

namespace Spring.Validation
{
    /// <summary>
    /// Unit tests for the AnyValidatorGroup class.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: AnyValidatorGroupTests.cs,v 1.4 2008/02/05 20:40:26 aseovic Exp $</version>
    [TestFixture]
    public sealed class AnyValidatorGroupTests
    {
        [Test]
        public void WhenAllValidatorsReturnFalse()
        {
            AnyValidatorGroup vg = new AnyValidatorGroup();
            vg.Validators.Add(new FalseValidator());
            vg.Validators.Add(new FalseValidator());
            vg.Validators.Add(new FalseValidator());

            IValidationErrors errors = new ValidationErrors();
            errors.AddError("existingErrors", new ErrorMessage("error", null));

            bool valid = vg.Validate(new object(), errors);

            Assert.IsFalse(valid, "Validation should fail when all inner validators return false.");
            Assert.AreEqual(3, errors.GetErrors("errors").Count);
            Assert.AreEqual(1, errors.GetErrors("existingErrors").Count);
        }

        [Test]
        public void WhenAllValidatorsReturnTrue()
        {
            AnyValidatorGroup vg = new AnyValidatorGroup("true");
            vg.Validators.Add(new TrueValidator());
            vg.Validators.Add(new TrueValidator());
            vg.Validators.Add(new TrueValidator());

            IValidationErrors errors = new ValidationErrors();
            errors.AddError("existingErrors", new ErrorMessage("error", null));

            bool valid = vg.Validate(new object(), errors);

            Assert.IsTrue(valid, "Validation should succeed when all inner validators return true.");
            Assert.AreEqual(0, errors.GetErrors("errors").Count);
            Assert.AreEqual(1, errors.GetErrors("existingErrors").Count);
        }

        [Test]
        public void WhenSingleValidatorReturnsTrue()
        {
            AnyValidatorGroup vg = new AnyValidatorGroup(Expression.Parse("true"));
            vg.Validators.Add(new FalseValidator());
            vg.Validators.Add(new TrueValidator());
            vg.Validators.Add(new FalseValidator());

            IValidationErrors errors = new ValidationErrors();
            errors.AddError("existingErrors", new ErrorMessage("error", null));

            bool valid = vg.Validate(new object(), errors);

            Assert.IsTrue(valid, "Validation should succeed when single inner validator returns true.");
            Assert.AreEqual(0, errors.GetErrors("errors").Count);
            Assert.AreEqual(1, errors.GetErrors("existingErrors").Count);
        }

        [Test]
        public void WhenGroupIsNotValidatedBecauseWhenExpressionReturnsFalse()
        {
            AnyValidatorGroup vg = new AnyValidatorGroup("false");
            vg.Validators.Add(new FalseValidator());
            vg.Validators.Add(new FalseValidator());

            IValidationErrors errors = new ValidationErrors();
            errors.AddError("existingErrors", new ErrorMessage("error", null));

            bool valid = vg.Validate(new object(), errors);

            Assert.IsTrue(valid, "Validation should succeed when group validator is not evaluated.");
            Assert.AreEqual(0, errors.GetErrors("errors").Count);
            Assert.AreEqual(1, errors.GetErrors("existingErrors").Count);
        }

    }
}