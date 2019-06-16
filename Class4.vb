Namespace My
    Partial Friend Class MyApplication
        Private WithEvents DomainMaterial As AppDomain = AppDomain.CurrentDomain
        Private Function MaterialCheck(sender As Object, e As System.ResolveEventArgs) As System.Reflection.Assembly Handles DomainMaterial.AssemblyResolve
            If e.Name.Contains("MaterialSkin") Then
                Return System.Reflection.Assembly.Load(My.Resources.MaterialSkin)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
