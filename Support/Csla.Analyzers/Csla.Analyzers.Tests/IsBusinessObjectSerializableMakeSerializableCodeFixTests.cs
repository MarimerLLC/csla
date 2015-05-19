using Csla.Analyzers;
using Csla.Analyzers.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
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
	public sealed class IsBusinessObjectSerializableMakeSerializableCodeFixTests
	{
		[TestMethod]
		public void VerifyGetFixableDiagnosticIds()
		{
			var fix = new IsBusinessObjectSerializableMakeSerializableCodeFix();
			var ids = fix.FixableDiagnosticIds.ToList();

			Assert.AreEqual(1, ids.Count, nameof(ids.Count));
			Assert.AreEqual(IsBusinessObjectSerializableConstants.DiagnosticId, ids[0], 
				nameof(IsBusinessObjectSerializableConstants.DiagnosticId));
		}

		[TestMethod]
		public async Task VerifyGetFixes()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(IsBusinessObjectSerializableMakeSerializableCodeFixTests)}.{(nameof(this.VerifyGetFixes))}.cs");
			var document = TestHelpers.Create(code);
			var tree = await document.GetSyntaxTreeAsync();
			var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new IsBusinessObjectSerializableAnalyzer());
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var actions = new List<CodeAction>();
			var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
				(a, _) => { actions.Add(a); });

			var fix = new IsBusinessObjectSerializableMakeSerializableCodeFix();
			var codeFixContext = new CodeFixContext(document, diagnostics[0],
				codeActionRegistration, new CancellationToken(false));
			await fix.RegisterCodeFixesAsync(codeFixContext);

			Assert.AreEqual(1, actions.Count);
			var action = actions[0];

			var operation = (await action.GetOperationsAsync(
				new CancellationToken(false))).ToArray()[0] as ApplyChangesOperation;
			var newDoc = operation.ChangedSolution.GetDocument(document.Id);
			var newTree = await newDoc.GetSyntaxTreeAsync();
			var changes = newTree.GetChanges(tree);

			Assert.AreEqual(IsBusinessObjectSerializableMakeSerializableCodeFixConstants.Description, action.Title,
				nameof(action.Title));
			Assert.AreEqual(1, changes.Count, nameof(changes.Count));
			Assert.AreEqual($"{Environment.NewLine}[Serializable]", changes[0].NewText, nameof(TextChange.NewText));
		}
	}
}
