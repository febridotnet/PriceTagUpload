Imports System.Configuration
Imports System.Security.Cryptography
Imports System.Text

Module ConfigHelper
    Private ReadOnly Entropy() As Byte = Encoding.UTF8.GetBytes("PriceTagUpload@2026")

    Function GetDecryptedConnectionString() As String
        Dim raw = ConfigurationManager.ConnectionStrings("PriceTagDb").ConnectionString

        If raw.StartsWith("data source", StringComparison.OrdinalIgnoreCase) Then
            Return raw
        End If

        Try
            Dim encryptedBytes = Convert.FromBase64String(raw)
            Dim decryptedBytes = ProtectedData.Unprotect(encryptedBytes, Entropy, DataProtectionScope.CurrentUser)
            Return Encoding.UTF8.GetString(decryptedBytes)
        Catch
            Return raw
        End Try
    End Function

    Sub SaveNewConnectionString(plainConnStr As String)
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim cs = config.ConnectionStrings.ConnectionStrings("PriceTagDb")

        Dim plainBytes = Encoding.UTF8.GetBytes(plainConnStr)
        Dim encryptedBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser)
        cs.ConnectionString = Convert.ToBase64String(encryptedBytes)
        config.Save()
    End Sub

    Sub EncryptConnectionStringInConfig()
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim cs = config.ConnectionStrings.ConnectionStrings("PriceTagDb")
        Dim raw = cs.ConnectionString

        If Not raw.StartsWith("data source", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim plainBytes = Encoding.UTF8.GetBytes(raw)
        Dim encryptedBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser)
        cs.ConnectionString = Convert.ToBase64String(encryptedBytes)
        config.Save()
    End Sub
End Module
