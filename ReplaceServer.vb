Public Class ReplaceServer
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.CheckServerExist.Stop()
        System.IO.Directory.Delete("C:\HomeWin\Servers\FiveM", True)
        My.Computer.FileSystem.CreateDirectory("C:\HomeWin\Servers\FiveM")
        My.Computer.FileSystem.CopyDirectory("C:\HomeWin\Servers\Backup\FiveM", "C:\HomeWin\Servers\FiveM", True)
        Form1.HideAlertBox.Start()
        Form1.CheckServerExist.Start()
        Me.Close()
    End Sub
End Class