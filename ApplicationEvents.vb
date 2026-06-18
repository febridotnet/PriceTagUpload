Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    Partial Friend Class MyApplication
        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Try
                If Environment.GetCommandLineArgs().Length > 1 AndAlso
                   Environment.GetCommandLineArgs()(1).Equals("/reconfig", StringComparison.OrdinalIgnoreCase) Then

                    Using frm As New FrmConfig()
                        frm.ShowDialog()
                    End Using

                    MessageBox.Show("Connection string has been updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
