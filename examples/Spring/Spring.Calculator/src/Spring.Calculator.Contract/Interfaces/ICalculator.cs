#region License

/*
 * Copyright � 2002-2006 the original author or authors.
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

using Spring.Calculator.Domain;

#endregion

namespace Spring.Calculator.Interfaces
{
	/// <summary>
	/// A simple calculator service interface.
	/// </summary>
    /// <author>Bruno Baia</author>
	/// <version>$Id: ICalculator.cs,v 1.1 2006/10/29 18:39:04 bbaia Exp $</version>
    public interface ICalculator
    {
        int Add(int n1, int n2);

        int Substract(int n1, int n2);

        DivisionResult Divide(int n1, int n2);

        int Multiply(int n1, int n2);
    }
}