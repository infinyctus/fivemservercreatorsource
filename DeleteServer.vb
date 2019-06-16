Public Class DeleteServer
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        System.IO.Directory.Delete("C:\HomeWin\Servers\FiveM", True)
        Application.Exit()
    End Sub
End Class