﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.InvokeDelegateWithConditionalAccess;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Diagnostics.InvokeDelegateWithConditionalAccess
{
    public class InvokeDelegateWithConditionalAccessTests : AbstractCSharpDiagnosticProviderBasedUserDiagnosticTest
    {
        internal override Tuple<DiagnosticAnalyzer, CodeFixProvider> CreateDiagnosticProviderAndFixer(Workspace workspace)
        {
            return new Tuple<DiagnosticAnalyzer, CodeFixProvider>(
                new InvokeDelegateWithConditionalAccessAnalyzer(),
                new InvokeDelegateWithConditionalAccessCodeFixProvider());
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.CodeActionsInvokeDelegateWithConditionalAccess)]
        public void Test1()
        {
            Test(
@"class C
{
    System.Action a;
    void Foo()
    {
        [||]var v = a;
        if (v != null)
        {
            v();
        }
    }
}",
@"
class C
{
    System.Action a;
    void Foo()
    {
        a?.Invoke();
    }
}");
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.CodeActionsInvokeDelegateWithConditionalAccess)]
        public void TestInvertedIf()
        {
            Test(
@"class C
{
    System.Action a;
    void Foo()
    {
        [||]var v = a;
        if (null != v)
        {
            v();
        }
    }
}",
@"
class C
{
    System.Action a;
    void Foo()
    {
        a?.Invoke();
    }
}");
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.CodeActionsInvokeDelegateWithConditionalAccess)]
        public void TestIfWithNoBraces()
        {
            Test(
@"class C
{
    System.Action a;
    void Foo()
    {
        [||]var v = a;
        if (null != v)
            v();
    }
}",
@"
class C
{
    System.Action a;
    void Foo()
    {
        a?.Invoke();
    }
}");
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.CodeActionsInvokeDelegateWithConditionalAccess)]
        public void TestWithComplexExpression()
        {
            Test(
@"class C
{
    System.Action a;
    void Foo()
    {
        bool b = true;
        [||]var v = b ? a : null;
        if (v != null)
        {
            v();
        }
    }
}",
@"
class C
{
    System.Action a;
    void Foo()
    {
        bool b = true;
        (b ? a : null)?.Invoke();
    }
}");
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.CodeActionsInvokeDelegateWithConditionalAccess)]
        public void TestMissingWithElseClause()
        {
            TestMissing(
@"class C
{
    System.Action a;
    void Foo()
    {
        [||]var v = a;
        if (v != null)
        {
            v();
        }
        else {}
    }
}");
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.CodeActionsInvokeDelegateWithConditionalAccess)]
        public void TestMissingWithMultipleVariables()
        {
            TestMissing(
@"class C
{
    System.Action a;
    void Foo()
    {
        [||]var v = a, x = a;
        if (v != null)
        {
            v();
        }
        else {}
    }
}");
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.CodeActionsInvokeDelegateWithConditionalAccess)]
        public void TestMissingIfUsedOutside()
        {
            TestMissing(
@"class C
{
    System.Action a;
    void Foo()
    {
        [||]var v = a;
        if (v != null)
        {
            v();
        }

        v = null;
    }
}");
        }
    }
}
