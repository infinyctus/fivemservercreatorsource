Namespace My
    Partial Friend Class MyApplication
        Private WithEvents Domain As AppDomain = AppDomain.CurrentDomain
        Private Function DomainBunifuCheck(sender As Object, e As System.ResolveEventArgs) As System.Reflection.Assembly Handles Domain.AssemblyResolve
            If e.Name.Contains("MySql.Data") Then
                Return System.Reflection.Assembly.Load(My.Resources.MySql_Data)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
