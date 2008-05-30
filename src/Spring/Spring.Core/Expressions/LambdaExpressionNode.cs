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

using System;
using System.Collections;
using System.Runtime.Serialization;
using antlr.collections;

namespace Spring.Expressions
{
    /// <summary>
    /// Represents lambda expression.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: LambdaExpressionNode.cs,v 1.8 2007/09/07 03:01:26 markpollack Exp $</version>
    [Serializable]
    public class LambdaExpressionNode : BaseNode
    {
        /// <summary>
        /// caches argumentNames of this instance
        /// </summary>
        private string[] argumentNames;
        
        /// <summary>
        /// caches body expression of this lambda function
        /// </summary>
        private BaseNode bodyExpression;

        /// <summary>
        /// Create a new instance
        /// </summary>
        public LambdaExpressionNode()
        {
        }

        /// <summary>
        /// Create a new instance from SerializationInfo
        /// </summary>
        protected LambdaExpressionNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        /// <summary>
        /// Gets argument names for this lambda expression.
        /// </summary>
        public string[] ArgumentNames
        {
            get
            {
                if(bodyExpression == null)
                {
                    InitializeLambda();
                }
                return argumentNames;
            }
        }

        /// <summary>
        /// Assigns value of the right operand to the left one.
        /// </summary>
        /// <param name="context">Context to evaluate expressions against.</param>
        /// <param name="evalContext">Current expression evaluation context.</param>
        /// <returns>Node's value.</returns>
        protected override object Get(object context, EvaluationContext evalContext)
        {
            if(bodyExpression == null)
            {
                InitializeLambda();
            }

            object result = bodyExpression.GetValueInternal(context, evalContext);
            return result;
        }

        /// <summary>
        /// Returns Lambda Expression's value for the given context.
        /// </summary>
        /// <param name="context">Context to evaluate expressions against.</param>
        /// <param name="evalContext">Current expression evaluation context.</param>
        /// <param name="arguments">A dictionary containing argument map for this lambda expression.</param>
        /// <returns>Node's value.</returns>
        public object GetValueInternal(object context, EvaluationContext evalContext, IDictionary arguments)
        {
            using (evalContext.SwitchLocalVariables(arguments))
            {
                object result = base.GetValueInternal(context, evalContext);
                return result;
            }
        }
        
        private void InitializeLambda()
        {
            lock (this)
            {
                if (bodyExpression == null)
                {
                    if (this.getNumberOfChildren() == 1)
                    {
                        argumentNames = new string[0];
                        bodyExpression = (BaseNode)this.getFirstChild();
                    }
                    else
                    {
                        AST argsNode = this.getFirstChild();
                        argumentNames = new string[argsNode.getNumberOfChildren()];
                        AST argNode = argsNode.getFirstChild();
                        int i = 0;
                        while (argNode != null)
                        {
                            argumentNames[i++] = argNode.getText();
                            argNode = argNode.getNextSibling();
                        }

                        bodyExpression = (BaseNode)argsNode.getNextSibling();
                    }
                }
            }
        }
    }
}