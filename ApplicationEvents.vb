Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    Partial Friend Class MyApplication
        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Try
                Dim args = Environment.GetCommandLineArgs()
                If args.Length > 1 AndAlso args(1).Equals("/reconfig", StringComparison.OrdinalIgnoreCase) Then

                    Using frm As New FrmConfig()
                        If frm.ShowDialog() = DialogResult.OK Then
                            MessageBox.Show("Connection string has been updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            Environment.Exit(0)
                        End If
                    End Using
                ElseIf args.Length > 1 AndAlso args(1).Equals("/SendToStore", StringComparison.OrdinalIgnoreCase) Then

                    Using frm As New FrmSendToStore()
                        frm.ShowDialog()
                    End Using
                    Environment.Exit(0)
                End If

                EncryptConnectionStringInConfig()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Environment.Exit(1)
            End Try
        End Sub
    End Class
End Namespace
