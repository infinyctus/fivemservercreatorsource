Namespace My
    Partial Friend Class MyApplication
        Private WithEvents DomainBunifu As AppDomain = AppDomain.CurrentDomain
        Private Function DomainCheck(sender As Object, e As System.ResolveEventArgs) As System.Reflection.Assembly Handles DomainBunifu.AssemblyResolve
            If e.Name.Contains("Bunifu_UI_v1.5.3") Then
                Return System.Reflection.Assembly.Load(My.Resources.Bunifu_UI_v1_5_3)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
