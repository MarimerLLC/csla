''' <summary>
''' Allows us to mark DataPortal_xxx methods to
''' be run on the client even if the server-side
''' DataPortal is configured for remote use.
''' </summary>
''' <remarks>
''' <para>
''' The primary purpose for this attribute is to
''' mark DataPortal_Create to run locally in the case
''' where we don't need to load default values
''' from the database as the object is being created.
''' </para><para>
''' By running DataPortal_Create locally we avoid all
''' the network overhead of going to the server for
''' no purpose.
''' </para><para>
''' <b>Note that if you DO need to actually interact with
''' the database in your DataPortal_xxx method you SHOULD
''' NOT apply this attribute to your DataPortal_xxx method!</b>
''' </para><para>
''' Also note that if you apply this attribute and the 
''' Transactional attribute to the same method, you MUST
''' register the ServicedDataPortal DLL with COM+ on the
''' client machine or you'll get a runtime failure. The
''' exception to this is if the user is an administrator
''' on the client machine, in which case Enterprise Services
''' will automatically register the DLL with COM+.
''' </para>
''' </remarks>
<AttributeUsage(AttributeTargets.Method)> _
Public NotInheritable Class RunLocalAttribute
  Inherits Attribute

End Class
