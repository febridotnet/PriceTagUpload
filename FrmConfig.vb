Imports System.Windows.Forms

Public Class FrmConfig
    Inherits Form

    Private txtConn As TextBox
    Private btnOk As Button
    Private btnCancel As Button
    Private lblInfo As Label

    Public Sub New()
        InitializeComponents()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Reconfigure Connection String"
        Me.ClientSize = New Size(650, 250)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        lblInfo = New Label With {
            .Text = "Enter the new connection string:",
            .Location = New Point(12, 12),
            .Size = New Size(600, 20)
        }

        txtConn = New TextBox With {
            .Location = New Point(12, 38),
            .Size = New Size(610, 140),
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical,
            .Font = New Font("Consolas", 10),
            .Text = "data source=...;initial catalog=...;integrated security=false;user id=...;password=...;"
        }

        btnOk = New Button With {
            .Text = "Save && Encrypt",
            .Location = New Point(380, 195),
            .Size = New Size(150, 30)
        }
        AddHandler btnOk.Click, AddressOf BtnOk_Click

        btnCancel = New Button With {
            .Text = "Cancel",
            .Location = New Point(538, 195),
            .Size = New Size(90, 30)
        }
        AddHandler btnCancel.Click, AddressOf BtnCancel_Click

        Me.Controls.Add(lblInfo)
        Me.Controls.Add(txtConn)
        Me.Controls.Add(btnOk)
        Me.Controls.Add(btnCancel)
    End Sub

    Private Sub BtnOk_Click(sender As Object, e As EventArgs)
        Dim connStr = txtConn.Text.Trim()
        If String.IsNullOrWhiteSpace(connStr) Then
            MessageBox.Show("Connection string cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If Not connStr.StartsWith("data source", StringComparison.OrdinalIgnoreCase) AndAlso
           Not connStr.StartsWith("server", StringComparison.OrdinalIgnoreCase) Then
            Dim result = MessageBox.Show("The value does not look like a standard connection string. Save anyway?",
                                         "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If result <> DialogResult.Yes Then Return
        End If

        SaveNewConnectionString(connStr)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
