Imports System.Data.SqlClient
Imports System.Data
Imports System.Threading.Tasks
Imports System.Linq

Public Class FrmSendToStore
    Inherits Form

    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents btnSendToStore As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblProgress As Label

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Send to Store"
        Me.ClientSize = New Size(800, 500)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        DataGridView1 = New DataGridView With {
            .Location = New Point(12, 12),
            .Size = New Size(760, 410),
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .RowHeadersWidth = 51
        }

        btnSendToStore = New Button With {
            .Text = "SendToStore",
            .Location = New Point(12, 435),
            .Size = New Size(120, 35)
        }

        btnCancel = New Button With {
            .Text = "Cancel",
            .Location = New Point(140, 435),
            .Size = New Size(90, 35)
        }

        lblProgress = New Label With {
            .Text = "",
            .Location = New Point(250, 440),
            .Size = New Size(520, 25),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }

        Me.Controls.Add(DataGridView1)
        Me.Controls.Add(btnSendToStore)
        Me.Controls.Add(btnCancel)
        Me.Controls.Add(lblProgress)
    End Sub

    Private Sub FrmSendToStore_Load(sender As Object, e As EventArgs) Handles Me.Load
        LoadPendingData()
    End Sub

    Private Sub LoadPendingData()
        Try
            Dim connStr = GetDecryptedConnectionString()
            Using conn As New SqlConnection(connStr)
                conn.Open()
                Using cmd As New SqlCommand("select * from RMS_DataInit.dbo.VW_PriceTagUploadDataPending order by Last_Update_Date desc", conn)
                    Dim dt As New DataTable()
                    dt.Load(cmd.ExecuteReader())
                    dt.Columns.Add("Status", GetType(String)).DefaultValue = "Pending"
                    dt.Columns("Status").SetOrdinal(0)
                    DataGridView1.DataSource = dt
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Sub BtnSendToStore_Click(sender As Object, e As EventArgs) Handles btnSendToStore.Click
        Dim dt = TryCast(DataGridView1.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            MessageBox.Show("No data to process.", "Send to Store", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim centralConnStr = GetDecryptedConnectionString()
        Dim storeToIps As New Dictionary(Of String, List(Of String))
        Dim storeValues As New List(Of String)

        For Each row In dt.Rows
            Dim s = row("Store").ToString().Trim()
            If Not storeValues.Contains(s) Then storeValues.Add(s)
        Next

        Try
            Using conn As New SqlConnection(centralConnStr)
                conn.Open()
                For Each storeVal In storeValues
                    Dim sql As String
                    If storeVal = "0" Then
                        sql = "select RSIM2_IP from RMS_DataInit..VW_ActiveStore"
                    Else
                        sql = "select RSIM2_IP from RMS_DataInit..VW_ActiveStore where Store_No = " & storeVal
                    End If
                    Dim ips As New List(Of String)
                    Using cmd As New SqlCommand(sql, conn)
                        Using reader = cmd.ExecuteReader()
                            While reader.Read()
                                ips.Add(reader("RSIM2_IP").ToString().Trim())
                            End While
                        End Using
                    End Using
                    storeToIps(storeVal) = ips
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("Error querying store IPs: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Dim username = Environment.UserName
        Dim lockObj As New Object()
        Dim processed = 0
        Dim total = dt.Rows.Count

        btnSendToStore.Enabled = False

        Dim rows = dt.Rows.Cast(Of DataRow)().ToList()

        Await Task.Run(Sub()
                           Parallel.ForEach(rows, Sub(row)
                                                      Dim storeVal = row("Store").ToString().Trim()
                                                      Dim ips As List(Of String) = Nothing
                                                      If Not storeToIps.TryGetValue(storeVal, ips) OrElse ips.Count = 0 Then
                                                          UpdateRowStatus(row, "No IP")
                                                          SyncLock lockObj
                                                              processed += 1
                                                              UpdateProgress(processed, total)
                                                          End SyncLock
                                                          Return
                                                      End If

                                                      UpdateRowStatus(row, "Processing...")

                                                      Dim dateFrom = GetColumnValue(row, {"Date_From", "date_from", "DATE_FROM"})
                                                      Dim dateEnd = GetColumnValue(row, {"Date_End", "date_end", "DATE_END"})
                                                      Dim plu = GetColumnValue(row, {"PLU", "Plu", "plu"})
                                                      Dim promoDesc = GetColumnValue(row, {"Promo_Desc", "PROMO_DESCRIPTION", "promo_desc"})
                                                      Dim promoPrice = GetColumnValue(row, {"Promo_Price", "PROMO_PRICE", "promo_price"})
                                                      Dim promoMember = GetColumnValue(row, {"Promo_Member", "PROMO_MEMBER", "promo_member"})

                                                      Dim success As Boolean = True
                                                      For Each ip In ips
                                                          Dim storeConnStr = "data source=" & ip & ";initial catalog=RMS_DataInit;MultipleActiveResultSets=True;integrated security=false;user id=sa;password=bboey;"
                                                          Dim sSql = "Insert Into StoreSystem.dbo.PriceTag_Promotion Values(" &
                                "'" & dateFrom & "','" & dateEnd & "','" & plu &
                                "','" & promoDesc & "','" & promoPrice & "','" & promoMember &
                                "','" & Date.Now & "','" & username & "')"
                                                          Try
                                                              Using storeConn As New SqlConnection(storeConnStr)
                                                                  storeConn.Open()
                                                                  Using cmd As New SqlCommand(sSql, storeConn)
                                                                      cmd.ExecuteNonQuery()
                                                                  End Using
                                                              End Using
                                                              Using logConn As New SqlConnection(centralConnStr)
                                                                  logConn.Open()
                                                                  Using cmd As New SqlCommand("Insert Into RMS_DataInit.dbo.PriceTagUploadDataSentStatus Values('" & Replace(sSql, "'", "''") & "', 'sent', GETDATE())", logConn)
                                                                      cmd.ExecuteNonQuery()
                                                                  End Using
                                                              End Using
                                                          Catch ex As Exception
                                                              success = False
                                                              Try
                                                                  Using logConn As New SqlConnection(centralConnStr)
                                                                      logConn.Open()
                                                                      Using cmd As New SqlCommand("Insert Into RMS_DataInit.dbo.PriceTagUploadDataSentStatus Values('" & Replace(sSql, "'", "''") & "', 'failed', GETDATE())", logConn)
                                                                          cmd.ExecuteNonQuery()
                                                                      End Using
                                                                  End Using
                                                              Catch
                                                              End Try
                                                          End Try
                                                      Next

                                                      UpdateRowStatus(row, If(success, "Sent", "Failed"))

                                                      SyncLock lockObj
                                                          processed += 1
                                                          UpdateProgress(processed, total)
                                                      End SyncLock
                                                  End Sub)
                       End Sub)

        btnSendToStore.Enabled = True
        lblProgress.Text = "Complete: " & processed & " of " & total & " rows"
        MessageBox.Show("Processing complete. " & processed & " rows processed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub UpdateRowStatus(row As DataRow, status As String)
        If DataGridView1.InvokeRequired Then
            DataGridView1.Invoke(Sub() UpdateRowStatus(row, status))
            Return
        End If
        row("Status") = status
        DataGridView1.Refresh()
    End Sub

    Private Sub UpdateProgress(processed As Integer, total As Integer)
        If lblProgress.InvokeRequired Then
            lblProgress.Invoke(Sub() UpdateProgress(processed, total))
            Return
        End If
        lblProgress.Text = "Processing " & processed & " of " & total & " rows..."
    End Sub

    Private Function GetColumnValue(row As DataRow, possibleNames As String()) As String
        For Each name As String In possibleNames
            If row.Table.Columns.Contains(name) Then
                Dim val = row(name).ToString().Trim()
                Return val
            End If
        Next
        Return ""
    End Function

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        End
    End Sub
End Class
