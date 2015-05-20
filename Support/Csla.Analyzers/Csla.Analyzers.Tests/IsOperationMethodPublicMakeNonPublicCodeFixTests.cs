using Csla.Analyzers;
using Csla.Analyzers.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FixingIsOneWay.Tests
{
	[TestClass]
	public sealed class IsOperationMethodPublicMakeNonPublicCodeFixTests
	{
		[TestMethod]
		public void VerifyGetFixableDiagnosticIds()
		{
			var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
			var ids = fix.FixableDiagnosticIds.ToList();

			Assert.AreEqual(1, ids.Count, nameof(ids.Count));
			Assert.AreEqual(IsOperationMethodPublicAnalyzerConstants.DiagnosticId, ids[0], 
				nameof(IsOperationMethodPublicAnalyzerConstants.DiagnosticId));
		}

		[TestMethod]
		public async Task VerifyGetFixesWhenClassIsNotSealed()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(IsOperationMethodPublicMakeNonPublicCodeFixTests)}.{(nameof(this.VerifyGetFixesWhenClassIsNotSealed))}.cs");
			var document = TestHelpers.Create(code);
			var tree = await document.GetSyntaxTreeAsync();
			var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsOperationMethodPublicAnalyzer());
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var actions = new List<CodeAction>();
			var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
				(a, _) => { actions.Add(a); });

			var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
			var codeFixContext = new CodeFixContext(document, diagnostics[0],
				codeActionRegistration, new CancellationToken(false));
			await fix.RegisterCodeFixesAsync(codeFixContext);

			Assert.AreEqual(3, actions.Count);
			await TestHelpers.VerifyActionAsync(actions,
				IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.PrivateDescription, document,
				tree, "rivate");
			await TestHelpers.VerifyActionAsync(actions,
				IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.ProtectedDescription, document,
				tree, "rotected");
			await TestHelpers.VerifyActionAsync(actions,
				IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.InternalDescription, document,
				tree, "internal");
		}

		[TestMethod]
		public async Task VerifyGetFixesWhenClassIsSealed()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(IsOperationMethodPublicMakeNonPublicCodeFixTests)}.{(nameof(this.VerifyGetFixesWhenClassIsSealed))}.cs");
			var document = TestHelpers.Create(code);
			var tree = await document.GetSyntaxTreeAsync();
			var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsOperationMethodPublicAnalyzer());
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var actions = new List<CodeAction>();
			var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
				(a, _) => { actions.Add(a); });

			var fix = new IsOperationMethodPublicMakeNonPublicCodeFix();
			var codeFixContext = new CodeFixContext(document, diagnostics[0],
				codeActionRegistration, new CancellationToken(false));
			await fix.RegisterCodeFixesAsync(codeFixContext);

			Assert.AreEqual(2, actions.Count);
			await TestHelpers.VerifyActionAsync(actions,
				IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.PrivateDescription, document,
				tree, "rivate");
			await TestHelpers.VerifyActionAsync(actions,
				IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants.InternalDescription, document,
				tree, "internal");
		}
	}
}
