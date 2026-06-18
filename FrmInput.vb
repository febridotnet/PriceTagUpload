Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Globalization
Imports System.IO
Imports ExcelDataReader
Imports ExcelDataReader.Exceptions
Imports System.DirectoryServices
Imports System.Environment
Imports System.Configuration

Public Class FrmInput
    Dim dtc As DataTableCollection
    Dim dt, dt2 As DataTable
    Dim sSqlConn As SqlConnection
    Dim sSqlCmd As SqlCommand
    Dim sSqlDbConn As String
    Dim sUsername As String = System.Environment.UserName
    Dim sComputerName As String = System.Environment.MachineName
    Dim storeNames As Dictionary(Of String, String)

    Sub SqlConnect()
        sSqlDbConn = GetDecryptedConnectionString()
        sSqlConn = New SqlConnection(sSqlDbConn)
        If sSqlConn.State = ConnectionState.Closed Then sSqlConn.Open()
    End Sub
    Sub ResetData()
        MonthCalendar1.SelectionStart = Today
        MonthCalendar2.SelectionStart = MonthCalendar1.SelectionStart
        MonthCalendar1.MinDate = DateAdd(DateInterval.Day, 0, DateTime.Now).Date
        MonthCalendar2.MinDate = MonthCalendar1.MinDate
        txtStartDate.Text = MonthCalendar1.SelectionStart.ToString("dd/MMM/yyyy")
        txtEndDate.Text = MonthCalendar2.SelectionStart.ToString("dd/MMM/yyyy")
        txtPromoPeriod.Text = MonthCalendar1.SelectionStart.ToString("dd/MMM/yyyy") & " - " & MonthCalendar2.SelectionStart.ToString("dd/MMM/yyyy")
        DataGridView1.DataSource = ""
        DataGridView2.DataSource = ""
        txtFileName.Text = ""
        ProgressBar1.Value = 0
        txtTotalRec.Text = ""

    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Text = Me.Text & "     >>> " & sUsername & " - " & sComputerName & " <<< "
        ResetData()
        txtStartDate.Enabled = False
        txtEndDate.Enabled = False
        txtPromoPeriod.Enabled = False
        txtFileName.Enabled = False
        txtTotalRec.Enabled = False
    End Sub
    Private Sub btnFile_Click(sender As Object, e As EventArgs) Handles btnFile.Click
        SqlConnect()
        Dim MsgInfUpload As String = "Pastikan File Sudah Ditutup & Format Sudah Sesuai" & vbCrLf &
                                "   - PLU : Tidak Boleh Blank / Nol / Lebih Dari 7 Digits" & vbCrLf &
                                "   - PLU : Tidak Boleh Duplicate di Store yang Sama" & vbCrLf &
                                "   - STORE : Tidak Boleh Blank" & vbCrLf &
                                "   - PROMO DESCRIPTION : Jika Blank > Akan Menjadi Harga Normal" & vbCrLf &
                                "   - PROMO PRICE : Jika Blank / Nol > Akan Menjadi Harga Normal"
        MessageBox.Show(MsgInfUpload, "Upload File", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Using ofd = New OpenFileDialog() With {.Filter = "Excel Workbook|*.xlsx|Excel 97-2003 Workbook|*.xls"}
            If ofd.ShowDialog = DialogResult.OK Then
                txtFileName.Text = ofd.FileName
                Using stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read)
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance)
                    Using reader = ExcelReaderFactory.CreateReader(stream)
                        Dim result = reader.AsDataSet(New ExcelDataSetConfiguration() With {
                                                                 .ConfigureDataTable = Function(__) New ExcelDataTableConfiguration() With {
                                                                 .UseHeaderRow = True}})
                        dt = result.Tables(0)
                        DataGridView1.DataSource = dt
                        DataGridView1.Columns(0).HeaderText = "PLU"
                        DataGridView1.Columns(1).HeaderText = "PROMO DESCRIPTION"
                        DataGridView1.Columns(2).HeaderText = "PROMO PRICE"
                        DataGridView1.Columns(3).HeaderText = "PROMO MEMBER"
                        DataGridView1.ReadOnly = True

                        dt2 = result.Tables(1)
                        dt2 = dt2.DefaultView.ToTable(True, dt2.Columns(0).ColumnName)
                        DataGridView2.DataSource = dt2
                        DataGridView2.Columns(0).HeaderText = "STORE LOCATION"
                        DataGridView2.ReadOnly = True

                        ' If DataGridView2.Rows.Count > 0 Then
                        Dim sSql2 = "select a.Store_No,b.STR_NAME,b.COUNTY from OMMSDE.dbo.StoreList a left join WTCSG.dbo.STRMASTR b on a.Store_No = b.STR_NBR where RSIM2_Aktif = 1"
                        Dim dtStore As New DataTable
                        Dim jml = 0
                        sSqlCmd = New SqlCommand(sSql2, sSqlConn)
                        dtStore.Load(sSqlCmd.ExecuteReader)
                        storeNames = New Dictionary(Of String, String)
                        For Each row In dtStore.Rows
                            Dim key = row("Store_No").ToString.Trim
                            Dim val = row("STR_NAME").ToString.Trim
                            If Not storeNames.ContainsKey(key) Then
                                storeNames(key) = val
                            End If
                        Next
                        ' End If
                        Dim TotalRec As Double = dt.Rows.Count
                        txtTotalRec.Text = dt.Rows.Count.ToString("#,##0")
                    End Using
                End Using
            End If
        End Using
    End Sub

    Private Sub MonthCalendar1_DateChanged(sender As Object, e As DateRangeEventArgs) Handles MonthCalendar1.DateChanged
        txtStartDate.Text = MonthCalendar1.SelectionStart.ToString("dd/MMM/yyyy")
        MonthCalendar2.MinDate = MonthCalendar1.SelectionStart
        txtEndDate.Text = MonthCalendar2.SelectionStart.ToString("dd/MMM/yyyy")
        txtPromoPeriod.Text = MonthCalendar1.SelectionStart.ToString("dd/MMM/yyyy") & " - " & MonthCalendar2.SelectionStart.ToString("dd/MMM/yyyy")

    End Sub
    Private Sub MonthCalendar2_DateChanged(sender As Object, e As DateRangeEventArgs) Handles MonthCalendar2.DateChanged
        txtEndDate.Text = MonthCalendar2.SelectionStart.ToString("dd/MMM/yyyy")
        txtPromoPeriod.Text = MonthCalendar1.SelectionStart.ToString("dd/MMM/yyyy") & " - " & MonthCalendar2.SelectionStart.ToString("dd/MMM/yyyy")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        SqlConnect()
        Dim X = DataGridView2.Rows(0).Cells(0).FormattedValue

        Dim sSql As String
        Dim dt = DataGridView1.DataSource
        Dim dt2 = DataGridView2.DataSource

        Try
            sSql = "Create Table #PriceTagUploadData (Date_From Datetime Null,Date_End Datetime Null,Plu Varchar(7) Null,Promo_Desc  Varchar(100) Null,Promo_Price  Varchar(18) Null, 
                    Promo_Member Varchar(7) Null,Store Varchar(7) Null,Last_Update_Date Datetime Null,Last_Update_By Varchar(100) Null)"
            sSql = "Truncate Table RMS_DataInit.dbo.PriceTagUploadDataRaw"
            sSqlCmd = New SqlCommand(sSql, sSqlConn)
            sSqlCmd.ExecuteNonQuery()

            For i = 0 To dt.Rows.Count - 1
                If (X = "STORE NOT FOUND") Then
                    MsgBox("Store is not registered!", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Store Not Found")
                    Exit Sub
                End If

                sSql = "Insert Into RMS_DataInit.dbo.PriceTagUploadDataRaw Values(" &
                    "'" & MonthCalendar1.SelectionStart.ToString("d") & "','" & MonthCalendar2.SelectionStart.ToString("d") & "','" & dt.Rows(i).Item("PLU").ToString.Trim &
                    "','" & dt.Rows(i).Item("PROMO_DESCRIPTION").ToString.Trim & "','" & dt.Rows(i).Item("PROMO_PRICE").ToString.Trim & "','" & dt.Rows(i).Item("PROMO_MEMBER").ToString.Trim &
                    "','" & dt2.Rows(0).Item("STORE").ToString.Trim & "','','" & sUsername & "')"
                sSqlCmd = New SqlCommand(sSql, sSqlConn)
                sSqlCmd.ExecuteNonQuery()

                ProgressBar1.Value = ((i + 1) / (dt.Rows.Count)) * 100
            Next

            sSql = "Exec RMS_DataInit.dbo.Sp_Upload_Data_PriceTag"
            sSqlCmd = New SqlCommand(sSql, sSqlConn)
            sSqlCmd.ExecuteNonQuery()

            MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ResetData()
        Catch exError As Exception
            MessageBox.Show("Format Tidak Sesuai", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            MessageBox.Show(exError.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridView2_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView2.CellFormatting
        If e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 AndAlso e.Value IsNot Nothing Then
            Dim val = e.Value.ToString().Trim()
            If val = "0" Then
                e.Value = "ALL STORE"
                e.FormattingApplied = True
            ElseIf storeNames IsNot Nothing AndAlso storeNames.ContainsKey(val) Then
                e.Value = storeNames(val)
                e.FormattingApplied = True
            ElseIf storeNames.ContainsKey(val) = False AndAlso val <> "" Then
                e.Value = "STORE NOT FOUND"
                e.FormattingApplied = True
            End If
        End If
    End Sub

End Class
