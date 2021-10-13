//-----------------------------------------------------------------------
// <copyright file="SerializationPartialBuilder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Builds the text of a partial type to implement the IMobileObject interface</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Csla.Generators.CSharp.AutoSerialization
{

  /// <summary>
  /// Builds the text of a partial type to implement the IMobileObject interface
  /// so that the target type automatically offers mobile serialization
  /// </summary>
  internal class SerializationPartialBuilder
  {

    /// <summary>
    /// Build the text of a partial type that implements the IMobileObject
    /// interface so that the target type automatically offers mobile serialization
    /// </summary>
    /// <param name="typeDefinition">The definition of the type for which serialization is required</param>
    /// <returns>Generated code to fulfil the required serialization</returns>
    internal GenerationResults BuildPartialTypeDefinition(ExtractedTypeDefinition typeDefinition)
    {
      GenerationResults generationResults;
      IndentedTextWriter textWriter;

      using (StringWriter stringWriter = new StringWriter())
      {
        textWriter = new IndentedTextWriter(stringWriter, "\t");

        AppendUsingStatements(textWriter, typeDefinition);
        AppendContainerDefinitions(textWriter, typeDefinition);

        AppendTypeDefinition(textWriter, typeDefinition);
        AppendBlockStart(textWriter);

        AppendGetChildrenMethod(textWriter, typeDefinition);
        AppendGetStateMethod(textWriter, typeDefinition);
        AppendSetChildrenMethod(textWriter, typeDefinition);
        AppendSetStateMethod(textWriter, typeDefinition);

        AppendBlockEnd(textWriter);

        AppendContainerDefinitionClosures(textWriter, typeDefinition);

        generationResults = new GenerationResults()
        {
          FullyQualifiedName = typeDefinition.FullyQualifiedName,
          GeneratedSource = stringWriter.ToString()
        };
      }

      return generationResults;
    }

    #region Private Helper Methods

    /// <summary>
    /// Append the start of a code block, indenting the writer
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the block start</param>
    private void AppendBlockStart(IndentedTextWriter textWriter)
    {
      textWriter.WriteLine("{");
      textWriter.Indent++;
    }

    /// <summary>
    /// Append the end of a code block, having first unindented the writer
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the block end</param>
    private void AppendBlockEnd(IndentedTextWriter textWriter)
    {
      textWriter.Indent--;
      textWriter.WriteLine("}");
    }

    /// <summary>
    /// Append the required using statements required on a partial class in
    /// order for it to compile the code we have generated
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the usings</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendUsingStatements(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      HashSet<string> requiredNamespaces;

      requiredNamespaces = GetRequiredNamespaces(typeDefinition);

      foreach (string requiredNamespace in requiredNamespaces)
      {
        textWriter.Write("using ");
        textWriter.Write(requiredNamespace);
        textWriter.WriteLine(";");
      }

      textWriter.WriteLine();
    }

    /// <summary>
    /// Retrieve all of the namespaces that are required for generation of the defined type
    /// </summary>
    /// <param name="typeDefinition">The definition of the type for which generation is being performed</param>
    /// <returns>A hashset of all of the namespaces required for generation</returns>
    private HashSet<string> GetRequiredNamespaces(ExtractedTypeDefinition typeDefinition)
    {
      HashSet<string> requiredNamespaces = new HashSet<string>() { "System", "Csla.Serialization.Mobile" };

      foreach (ExtractedFieldDefinition fieldDefinition in typeDefinition.Fields)
      {
        requiredNamespaces.Add(fieldDefinition.TypeDefinition.TypeNamespace);
      }

      foreach (ExtractedPropertyDefinition propertyDefinition in typeDefinition.Properties)
      {
        requiredNamespaces.Add(propertyDefinition.TypeDefinition.TypeNamespace);
      }

      requiredNamespaces.Remove(typeDefinition.Namespace);

      return requiredNamespaces;
    }

    /// <summary>
    /// Append the definitions of the containers in which the partial type is to reside
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the definitions</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendContainerDefinitions(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      foreach (ExtractedContainerDefinition containerDefinition in typeDefinition.ContainerDefinitions)
      {
        textWriter.WriteLine(containerDefinition.FullDefinition);
        AppendBlockStart(textWriter);
      }
    }

    /// <summary>
    /// Append the definitions of the containers in which the partial type is to reside
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the definitions</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendContainerDefinitionClosures(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      foreach (ExtractedContainerDefinition containerDefinition in typeDefinition.ContainerDefinitions)
      {
        AppendBlockEnd(textWriter);
      }
    }

    /// <summary>
    /// Append the type definition of the partial we are generating
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the type definition</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendTypeDefinition(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      textWriter.WriteLine("[Serializable]");
      textWriter.Write(typeDefinition.Scope);
      textWriter.Write(" partial ");
      textWriter.Write(typeDefinition.TypeKind);
      textWriter.Write(" ");
      textWriter.Write(typeDefinition.TypeName);
      textWriter.Write(" : IMobileObject");
      textWriter.WriteLine();
    }

    /// <summary>
    /// Append the definition of the GetChildren method required to fulfil the IMobileObject interface
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the method definition</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendGetChildrenMethod(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      textWriter.WriteLine("void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)");
      AppendBlockStart(textWriter);
      if (HasChildrenToExpose(typeDefinition))
      {
        textWriter.WriteLine("IMobileObject mobileObject;");
        textWriter.WriteLine("SerializationInfo childInfo;");
        textWriter.WriteLine();
      }

      foreach (ExtractedFieldDefinition fieldDefinition in typeDefinition.Fields)
      {
        if (!IsChildToExpose(fieldDefinition)) continue;

        AppendSerializeChildFragment(textWriter, fieldDefinition);
      }

      foreach (ExtractedPropertyDefinition propertyDefinition in typeDefinition.Properties)
      {
        if (!IsChildToExpose(propertyDefinition)) continue;

        AppendSerializeChildFragment(textWriter, propertyDefinition);
      }

      AppendBlockEnd(textWriter);
      textWriter.WriteLine();
    }

    /// <summary>
    /// Append the code fragment necessary to serialize an individual child member
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the fragment</param>
    /// <param name="memberDefinition">The definition of the member we are writing for</param>
    private void AppendSerializeChildFragment(IndentedTextWriter textWriter, IMemberDefinition memberDefinition)
    {
      textWriter.Write("mobileObject = ");
      textWriter.Write(memberDefinition.MemberName);
      textWriter.WriteLine(" as IMobileObject;");
      textWriter.WriteLine("childInfo = formatter.SerializeObject(mobileObject);");
      textWriter.Write("info.AddChild(nameof(");
      textWriter.Write(memberDefinition.MemberName);
      textWriter.WriteLine("), childInfo.ReferenceId, true);");
    }

    /// <summary>
    /// Append the definition of the SetChildren method required to fulfil the IMobileObject interface
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the method definition</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendSetChildrenMethod(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      textWriter.WriteLine("void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)");
      AppendBlockStart(textWriter);

      if (HasChildrenToExpose(typeDefinition))
      {
        textWriter.WriteLine("SerializationInfo.ChildData childData;");
        textWriter.WriteLine();
      }

      foreach (ExtractedFieldDefinition fieldDefinition in typeDefinition.Fields)
      {
        if (!IsChildToExpose(fieldDefinition)) continue;

        AppendDeserializeChildFragment(textWriter, fieldDefinition);
      }

      foreach (ExtractedPropertyDefinition propertyDefinition in typeDefinition.Properties)
      {
        if (!IsChildToExpose(propertyDefinition)) continue;

        AppendDeserializeChildFragment(textWriter, propertyDefinition);
      }

      AppendBlockEnd(textWriter);
      textWriter.WriteLine();
    }

    /// <summary>
    /// Append the code fragment necessary to deserialize an individual child member
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the fragment</param>
    /// <param name="memberDefinition">The definition of the member we are writing for</param>
    private void AppendDeserializeChildFragment(IndentedTextWriter textWriter, IMemberDefinition memberDefinition)
    {
      textWriter.Write("if (info.Children.ContainsKey(nameof(");
      textWriter.Write(memberDefinition.MemberName);
      textWriter.WriteLine(")))");
      AppendBlockStart(textWriter);

      textWriter.Write("childData = info.Children[nameof(");
      textWriter.Write(memberDefinition.MemberName);
      textWriter.WriteLine(")];");
      textWriter.WriteLine();
      textWriter.Write(memberDefinition.MemberName);
      textWriter.Write(" = formatter.GetObject(childData.ReferenceId) as ");
      textWriter.Write(memberDefinition.TypeDefinition.TypeName);
      textWriter.WriteLine(";");

      AppendBlockEnd(textWriter);
    }

    /// <summary>
    /// Append the definition of the GetState method required to fulfil the IMobileObject interface
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the method definition</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendGetStateMethod(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      textWriter.WriteLine("void IMobileObject.GetState(SerializationInfo info)");
      AppendBlockStart(textWriter);

      foreach (ExtractedFieldDefinition fieldDefinition in typeDefinition.Fields)
      {
        if (IsChildToExpose(fieldDefinition)) continue;

        AppendGetMemberStateFragment(textWriter, fieldDefinition);
      }

      foreach (ExtractedPropertyDefinition propertyDefinition in typeDefinition.Properties)
      {
        if (IsChildToExpose(propertyDefinition)) continue;

        AppendGetMemberStateFragment(textWriter, propertyDefinition);
      }

      AppendBlockEnd(textWriter);
      textWriter.WriteLine();
    }

    /// <summary>
    /// Append the code fragment necessary to get the state of an individual member
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the fragment</param>
    /// <param name="memberDefinition">The definition of the member we are writing for</param>
    private void AppendGetMemberStateFragment(IndentedTextWriter textWriter, IMemberDefinition memberDefinition)
    {
      textWriter.Write("info.AddValue(nameof(");
      textWriter.Write(memberDefinition.MemberName);
      textWriter.Write("), ");
      textWriter.Write(memberDefinition.MemberName);
      textWriter.WriteLine(");");
    }

    /// <summary>
    /// Append the definition of the SetState method required to fulfil the IMobileObject interface
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the method definition</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendSetStateMethod(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      textWriter.WriteLine("void IMobileObject.SetState(SerializationInfo info)");
      AppendBlockStart(textWriter);

      foreach (ExtractedFieldDefinition fieldDefinition in typeDefinition.Fields)
      {
        if (IsChildToExpose(fieldDefinition)) continue;

        AppendSetMemberStateMethod(textWriter, fieldDefinition);
      }

      foreach (ExtractedPropertyDefinition propertyDefinition in typeDefinition.Properties)
      {
        if (IsChildToExpose(propertyDefinition)) continue;

        AppendSetMemberStateMethod(textWriter, propertyDefinition);
      }

      AppendBlockEnd(textWriter);
      textWriter.WriteLine();
    }

    /// <summary>
    /// Append the code fragment necessary to set the state of an individual member
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the fragment</param>
    /// <param name="memberDefinition">The definition of the member we are writing for</param>
    private void AppendSetMemberStateMethod(IndentedTextWriter textWriter, IMemberDefinition memberDefinition)
    {
      textWriter.Write(memberDefinition.MemberName);
      textWriter.Write(" = info.GetValue<");
      textWriter.Write(memberDefinition.TypeDefinition.TypeName);
      textWriter.Write(">(nameof(");
      textWriter.Write(memberDefinition.MemberName);
      textWriter.WriteLine("));");
    }

    /// <summary>
    /// Determine if a type definition exposes any members that must be treated as children
    /// </summary>
    /// <param name="typeDefinition">The type definition to be checked for children</param>
    /// <returns>Boolean true of the definition exposes any members that have to be treated as children</returns>
    private bool HasChildrenToExpose(ExtractedTypeDefinition typeDefinition)
    {
      return typeDefinition.Properties.Any(p => IsChildToExpose(p)) ||
        typeDefinition.Fields.Any(f => IsChildToExpose(f));
    }

    /// <summary>
    /// Determine if a member definition defines a type that must be treated as a child
    /// </summary>
    /// <param name="memberDefinition">The definition of the member to be inspected</param>
    /// <returns>Boolean true if the definition is of a type that has to be treated as a child</returns>
    private bool IsChildToExpose(IMemberDefinition memberDefinition)
    {
      return memberDefinition.TypeDefinition.IsAutoSerializable || memberDefinition.TypeDefinition.ImplementsIMobileObject;
    }

    #endregion

  }

}
