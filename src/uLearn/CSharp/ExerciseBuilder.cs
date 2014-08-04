using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class ExerciseBuilder : CSharpSyntaxRewriter
	{
		public string ExerciseInitialCode { get; private set; }
		public bool IsExercise { get; private set; }
		public string ExpectedOutput { get; private set; }
		public readonly List<string> Hints = new List<string>();
		public MethodDeclarationSyntax ExerciseNode;

		public ExerciseBuilder()
			: base(false)
		{
		}

		private SyntaxNode VisitMemberDeclaration(MemberDeclarationSyntax node, SyntaxNode newNode)
		{
			var includeInSolution = !node.HasAttribute<ExcludeFromSolutionAttribute>() && !node.HasAttribute<ExerciseAttribute>();
			return includeInSolution ? ((MemberDeclarationSyntax) newNode).WithoutAttributes() : null;
		}

		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitClassDeclaration(node));
		}

		public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitEnumDeclaration(node));
		}

		public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitConstructorDeclaration(node));
		}

		public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitFieldDeclaration(node));
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			var newMethod = VisitMemberDeclaration(node, base.VisitMethodDeclaration(node));
			if (node.HasAttribute<ExpectedOutputAttribute>())
			{
				IsExercise = true;
				ExpectedOutput = node.GetAttributes<ExpectedOutputAttribute>().Select(attr => attr.GetArgument(0)).FirstOrDefault();
			}
			if (node.HasAttribute<HintAttribute>())
			{
				Hints.AddRange(node.GetAttributes<HintAttribute>().Select(attr => attr.GetArgument(0)));
			}
			if (node.HasAttribute<ExerciseAttribute>())
			{
				ExerciseNode = node;
				ExerciseInitialCode = GetExerciseCode(node);
			}
			return newMethod;
		}

		private string GetExerciseCode(MethodDeclarationSyntax method)
		{
			var codeLines = method.TransformExercise().ToPrettyString().SplitToLines();
			return string.Join("\n", FilterSpecialComments(codeLines));
		}

		private IEnumerable<string> FilterSpecialComments(IEnumerable<string> lines)
		{
			var inUncomment = false;
			foreach (var line in lines)
			{
				if (!inUncomment && line.Trim() == "/*uncomment")
				{
					inUncomment = true;
				}
				else if (inUncomment && line.Trim() == "*/")
				{
					inUncomment = false;
				}
				else
					yield return line;
			}
		}
	}
}