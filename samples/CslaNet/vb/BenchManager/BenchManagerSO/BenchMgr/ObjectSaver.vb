Imports Csla

Public Module ObjectSaver

  Public Function SaveObject(Of T As {Csla.Core.ISavable, ICloneable, Csla.Core.ISupportUndo})(ByVal obj As T) As T

    Dim result As T = obj
    Using New StatusBusy("Saving data...")
      Try
        Dim tmp As T = obj.Clone
        tmp.ApplyEdit()
        result = tmp.Save
        result.BeginEdit()

      Catch ex As DataPortalException
        MessageBox.Show(ex.Message, "Data error")
      Catch ex As Csla.Validation.ValidationException
        MessageBox.Show(ex.Message, "Data validation error")
      Catch ex As System.Security.SecurityException
        MessageBox.Show(ex.Message, "Security error")
      Catch ex As Exception
        MessageBox.Show(ex.ToString, "Unexpected error")
      End Try
    End Using

    ' return result
    Return result

  End Function

End Module
