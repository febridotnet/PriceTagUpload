Imports System.Data
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Data

Public Class FrmSendToStore
    Inherits Form

    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents btnSendToStore As Button
    Friend WithEvents btnRecheckActiveStore As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblProgress As Label
    Dim TotalGridRow As Integer

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString
        Me.Text = "Data Need to be Sent to Store - " & version
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
            .Text = "&SendToStore",
            .Location = New Point(12, 435),
            .Size = New Size(120, 35)
        }

        btnCancel = New Button With {
            .Text = "&Close",
            .Location = New Point(140, 435),
            .Size = New Size(90, 35)
        }

        btnRecheckActiveStore = New Button With {
            .Text = "Recheck Active Store",
            .Location = New Point(240, 435),
            .Size = New Size(200, 35),
            .Visible = False
        }

        lblProgress = New Label With {
            .Text = "",
            .Location = New Point(450, 440),
            .Size = New Size(370, 25),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }

        Me.Controls.Add(DataGridView1)
        Me.Controls.Add(btnSendToStore)
        Me.Controls.Add(btnRecheckActiveStore)
        Me.Controls.Add(btnCancel)
        Me.Controls.Add(lblProgress)
    End Sub

    Private Sub FrmSendToStore_Load(sender As Object, e As EventArgs) Handles Me.Load
        'btnSendToStore.Enabled = False
        LoadPendingData()
    End Sub

    Private Sub LoadPendingData()
        Try
            Dim connStr = GetDecryptedConnectionString()
            Using conn As New SqlConnection(connStr)
                conn.Open()
                'Using cmd As New SqlCommand("select * from RMS_DataInit.dbo.VW_PriceTagUploadDataPending order by Store,Last_Update_Date asc", conn)
                Using cmd As New SqlCommand("select a.Store_No as 'Store#', a.ip as 'IP',a.last_faileddate as 'Last Failed',a.last_successdate as 'Last Success',a.start_running_date as 'Next Running' from RMS_DataInit.dbo.VW_RunningDateSentToStore a left join RMS_DataInit..ActiveStoreNow b on a.Store_No=b.StoreNo", conn)
                    Dim dt As New DataTable()
                    dt.Load(cmd.ExecuteReader())
                    dt.Columns.Add("Sent Status", GetType(String)).DefaultValue = "Pending"
                    dt.Columns("Sent Status").SetOrdinal(0)
                    DataGridView1.DataSource = dt
                    TotalGridRow = dt.Rows.Count
                    lblProgress.Text = "Total " & TotalGridRow & " stores"
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        If e.ColumnIndex >= 0 AndAlso e.RowIndex >= 0 Then
            Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
            If colName = "status" AndAlso e.Value IsNot Nothing Then
                Dim status = e.Value.ToString().Trim().ToLower()
                If status = "timeout" Then
                    e.CellStyle.ForeColor = Color.White
                    e.CellStyle.BackColor = Color.Red
                    e.CellStyle.SelectionForeColor = Color.White
                    e.CellStyle.SelectionBackColor = Color.Red
                    e.FormattingApplied = True
                ElseIf status = "active" Then
                    e.CellStyle.ForeColor = Color.White
                    e.CellStyle.BackColor = Color.Green
                    e.CellStyle.SelectionForeColor = Color.White
                    e.CellStyle.SelectionBackColor = Color.Green
                    e.FormattingApplied = True
                End If
            End If
        End If
    End Sub

    Private Async Sub BtnRecheckActiveStore_Click(sender As Object, e As EventArgs) Handles btnRecheckActiveStore.Click
        btnSendToStore.Enabled = False
        lblProgress.Text = "Checking Connection to Active Store.."
        DataGridView1.Enabled = False
        btnRecheckActiveStore.Enabled = False

        Dim centralConnStr = GetDecryptedConnectionString()
        DataGridView1.DataSource = ""
        Await Task.Run(Sub()
                           Dim procedureName As String = "RMS_DataInit..SP_ACTIVESTORE_CHECK"
                           Dim resultValue As Object = Nothing

                           Using conn As New SqlConnection(centralConnStr)
                               Using cmd As New SqlCommand(procedureName, conn)
                                   cmd.CommandType = CommandType.StoredProcedure
                                   cmd.CommandTimeout = 0
                                   Try
                                       conn.Open()
                                       resultValue = cmd.ExecuteScalar()
                                       If resultValue IsNot DBNull.Value AndAlso resultValue IsNot Nothing Then
                                           Dim status As String = resultValue.ToString()
                                           MsgBox("Checking Server Done", vbInformation + vbOKOnly, "Success!")
                                       Else
                                           MessageBox.Show("Data tidak ditemukan atau bernilai NULL.")
                                       End If
                                   Catch ex As Exception
                                       'MessageBox.Show("Terjadi kesalahan: " & ex.Message)
                                   End Try
                               End Using
                           End Using
                       End Sub)

        DataGridView1.Enabled = True
        LoadPendingData()
        DataGridView1.Refresh()
        lblProgress.Text = ""
        btnSendToStore.Enabled = True
        btnRecheckActiveStore.Enabled = True
    End Sub

    Private Async Sub BtnSendToStore_Click(sender As Object, e As EventArgs) Handles btnSendToStore.Click
        Dim dt = TryCast(DataGridView1.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            MessageBox.Show("No data to process.", "Send to Store", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim centralConnStr = GetDecryptedConnectionString()
        Dim storeToIps As New Dictionary(Of String, List(Of Tuple(Of String, String)))
        Dim allStoreInfo As New Dictionary(Of String, String)
        Dim storeValues As New List(Of String)

        For Each row In dt.Rows
            Dim s = row("Store#").ToString().Trim()
            If Not storeValues.Contains(s) Then storeValues.Add(s)
        Next

        Try
            Using conn As New SqlConnection(centralConnStr)
                conn.Open()
                For Each storeVal In storeValues
                    Dim sql As String
                    If storeVal = "0" Then
                        sql = "select StoreNo,ip from RMS_DataInit.dbo.ActiveStoreNow"
                        'sql = "select Store_No, RSIM2_IP from RMS_DataInit..VW_ActiveStore"
                    Else
                        sql = "select StoreNo,ip from RMS_DataInit.dbo.ActiveStoreNow where StoreNo = " & storeVal
                        'sql = "select Store_No, RSIM2_IP from RMS_DataInit..VW_ActiveStore where Store_No = " & storeVal
                    End If
                    Dim ips As New List(Of Tuple(Of String, String))
                    Using cmd As New SqlCommand(sql, conn)
                        Using reader = cmd.ExecuteReader()
                            While reader.Read()
                                Dim storeNo = reader("StoreNo").ToString().Trim()
                                Dim ip = reader("ip").ToString().Trim()
                                ips.Add(Tuple.Create(storeNo, ip))
                                If storeVal = "0" AndAlso Not allStoreInfo.ContainsKey(storeNo) Then
                                    allStoreInfo(storeNo) = ip
                                End If
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

        '============= new process =============
        Await Task.Run(Sub()
                           Parallel.ForEach(rows, Sub(row)
                                                      Dim Status = GetColumnValue(row, {"Status", "status", "Status"})
                                                      Dim processedStore = GetColumnValue(row, {"Store", "Store#", "Store#"})

                                                      ' ==================== SENDDATA TO ACTIVESTORE ====================
                                                      Dim ipAddr = GetColumnValue(row, {"IP", "ip"})
                                                      Dim lastCheck = GetColumnValue(row, {"Next Running", "start_running_date"})
                                                      If Not String.IsNullOrWhiteSpace(ipAddr) AndAlso Not String.IsNullOrWhiteSpace(processedStore) Then
                                                          Try
                                                              Using conn As New SqlConnection(centralConnStr)
                                                                  conn.Open()
                                                                  Using cmd As New SqlCommand("RMS_DataInit.dbo.SP_SENDDATA_TO_ACTIVESTORE", conn)
                                                                      cmd.CommandTimeout = 0
                                                                      cmd.CommandType = CommandType.StoredProcedure
                                                                      cmd.Parameters.AddWithValue("@IP", ipAddr)
                                                                      cmd.Parameters.AddWithValue("@STORE", Convert.ToInt32(processedStore))
                                                                      cmd.Parameters.AddWithValue("@RUNNING_DATE", lastCheck)
                                                                      'cmd.Parameters.AddWithValue("@RUNNING_DATE", If(String.IsNullOrWhiteSpace(lastCheck), DateTime.Now.ToString("M/d/yyyy h:mm tt"), lastCheck))
                                                                      cmd.ExecuteNonQuery()
                                                                  End Using
                                                              End Using
                                                              UpdateRowStatus(row, "Sent")

                                                              Using conn As New SqlConnection(centralConnStr)
                                                                  conn.Open()
                                                                  Using cmd As New SqlCommand("RMS_DataInit.dbo.SP_LOG_UPDATEDATE_ACTIVESTORE", conn)
                                                                      cmd.CommandTimeout = 0
                                                                      cmd.CommandType = CommandType.StoredProcedure
                                                                      cmd.Parameters.AddWithValue("@IP", ipAddr)
                                                                      cmd.Parameters.AddWithValue("@STORE", Convert.ToInt32(processedStore))
                                                                      cmd.Parameters.AddWithValue("@STATUS", "sent")
                                                                      cmd.ExecuteNonQuery()
                                                                  End Using
                                                              End Using
                                                          Catch ex As Exception
                                                              UpdateRowStatus(row, "Failed")

                                                              Using conn As New SqlConnection(centralConnStr)
                                                                  conn.Open()
                                                                  Using cmd As New SqlCommand("RMS_DataInit.dbo.SP_LOG_UPDATEDATE_ACTIVESTORE", conn)
                                                                      cmd.CommandTimeout = 0
                                                                      cmd.CommandType = CommandType.StoredProcedure
                                                                      cmd.Parameters.AddWithValue("@IP", ipAddr)
                                                                      cmd.Parameters.AddWithValue("@STORE", Convert.ToInt32(processedStore))
                                                                      cmd.Parameters.AddWithValue("@STATUS", "failed")
                                                                      cmd.ExecuteNonQuery()
                                                                  End Using
                                                              End Using
                                                          End Try
                                                      Else
                                                          UpdateRowStatus(row, "No IP/Store")
                                                      End If
                                                      ' ===============================================================

                                                      SyncLock lockObj
                                                          processed += 1
                                                          UpdateProgress(processed, total)
                                                      End SyncLock
                                                  End Sub)
                       End Sub)
        '=======================================

        'LoadPendingData()
        DataGridView1.Refresh()
        btnSendToStore.Enabled = True
        lblProgress.Text = "Processed Data: " & processed & " of " & total & " stores"
        'MessageBox.Show("Processing complete. " & processed & " rows processed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        'Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub UpdateRowStatus(row As DataRow, status As String)
        If DataGridView1.InvokeRequired Then
            DataGridView1.Invoke(Sub() UpdateRowStatus(row, status))
            Return
        End If
        row("Sent Status") = status
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
        If MsgBox("Are you sure want to exit?", vbYesNo + vbCritical + vbDefaultButton1, "Quit?") = vbYes Then
            Me.DialogResult = DialogResult.Cancel
            End
        End If
    End Sub
End Class
