﻿/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System.Collections.Generic;
using MSAst = System.Linq.Expressions;

namespace Microsoft.Scripting.Debugging {
    using Ast = System.Linq.Expressions.Expression;

    /// <summary>
    /// Default implementation of IDebugThreadFactory, which uses DLR's RuntimeVariablesExpression for lifting locals.
    /// </summary>
    internal sealed class DefaultDebugThreadFactory : IDebugThreadFactory {
        public DebugThread CreateDebugThread(Microsoft.Scripting.Debugging.CompilerServices.DebugContext debugContext) {
            return new DefaultDebugThread(debugContext);
        }

        public MSAst.Expression CreatePushFrameExpression(MSAst.ParameterExpression functionInfo, MSAst.ParameterExpression debugMarker, IList<MSAst.ParameterExpression> locals, IList<VariableInfo> varInfos, MSAst.Expression runtimeThread) {
            MSAst.ParameterExpression[] args = new MSAst.ParameterExpression[2 + locals.Count];
            args[0] = functionInfo;
            args[1] = debugMarker;
            for (int i = 0; i < locals.Count; i++) {
                args[i + 2] = locals[i];
            }

            return Ast.Call(
                typeof(RuntimeOps).GetMethod("LiftVariables"),
                runtimeThread,
                Ast.RuntimeVariables(args)
            );
        }
    }
}