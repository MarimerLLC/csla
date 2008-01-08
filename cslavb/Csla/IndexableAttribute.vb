Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

''' <summary>
''' Marks a property to indicate that, when contained in a CSLA collection 
''' class, that an index should be built for the property that will
''' be used in LINQ queries
''' </summary>
''' <remarks>
''' Marking a variable with this attribute will cause any CSLA collection
''' class to create an internal index on this value.  Use this carefully, as
''' the more items are indexed, the slower add operations will be
''' </remarks>
<AttributeUsage(AttributeTargets.Property)> _
Public NotInheritable Class IndexableAttribute
  Inherits Attribute
  Private _indexMode As IndexModeEnum = IndexModeEnum.IndexModeOnDemand
  ''' <summary>
  ''' Allows user to determine how indexing will occur
  ''' </summary>
  Public Property IndexMode() As IndexModeEnum
    Get
      Return _indexMode
    End Get
    Set(ByVal value As IndexModeEnum)
      _indexMode = value
    End Set
  End Property
  ''' <summary>
  ''' Sets the property as indexable on demand
  ''' </summary>
  Public Sub New()
  End Sub
  ''' <summary>
  ''' Set the indexable property, along with it's index mode
  ''' </summary>
  Public Sub New(ByVal indexMode As IndexModeEnum)
    _indexMode = indexMode
  End Sub

End Class
