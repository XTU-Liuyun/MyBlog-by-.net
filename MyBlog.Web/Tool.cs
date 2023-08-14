using System.Data;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
namespace MyBlog.Web
{
	/// <summary>
	/// 工具
	/// </summary>
	public static class Tool
	{
		/// <summary>过滤SQL非法字符串

		/// 字符串长度不能超过40个,把前后空间都去除
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetSafeSQL(string value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			value = value.Trim();
			value = Tool.StringTruncat(value, 40, "");
			value = Regex.Replace(value, @";", string.Empty);
			value = Regex.Replace(value, @"'", string.Empty);
			value = Regex.Replace(value, @"&", string.Empty);
			value = Regex.Replace(value, @"%20", string.Empty);
			value = Regex.Replace(value, @"--", string.Empty);
			value = Regex.Replace(value, @"==", string.Empty);
			value = Regex.Replace(value, @"<", string.Empty);
			value = Regex.Replace(value, @">", string.Empty);
			value = Regex.Replace(value, @"%", string.Empty);
			return value;
		}
		///   
		///   </summary> 
		///   <param   name= "oldStr "> 需要截断的字符串 </param> 
		///   <param   name= "maxLength "> 字符串的最大长度 </param> 
		///   <param   name= "endWith "> 超过长度的后缀 </param> 
		///   <returns> 如果超过长度，返回截断后的新字符串加上后缀，否则，返回原字符串 </returns> 
		public static string StringTruncat(string oldStr, int maxLength, string endWith)
		{
			if (string.IsNullOrEmpty(oldStr))
				//   throw   new   NullReferenceException( "原字符串不能为空 "); 
				return oldStr + endWith;
			if (maxLength < 1)
				throw new Exception("返回的字符串长度必须大于[0] ");
			if (oldStr.Length > maxLength)
			{
				string strTmp = oldStr.Substring(0, maxLength);
				if (string.IsNullOrEmpty(endWith))
					return strTmp;
				else
					return strTmp + endWith;
			}
			return oldStr;
		}
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
		///   <summary>     
		///   </summary>   
		///   <param    name="NoHTML">包括HTML的源码</param>   
		///   <returns>已经去除后的文字</returns>   
		public static string GetNoHTMLString(string Htmlstring)
		{
			if(string.IsNullOrEmpty(Htmlstring))
			{
				return "";
			}
			//删除脚本   
			Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
			//删除HTML   
			Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
			Htmlstring.Replace("<", "");
			Htmlstring.Replace(">", "");
			Htmlstring.Replace("\r\n", "");
			
			return Htmlstring;
		}
		/// <summary>
		/// 检查是否为合法邮箱
		/// </summary>
		/// <returns></returns>
	    public static bool CheckEmail(string str)
		{
            if (string.IsNullOrWhiteSpace(str))
                return false;
            string[] emial = str.Split('@');
            if (emial.Length == 2 && !string.IsNullOrWhiteSpace(emial[0]) && !string.IsNullOrWhiteSpace(emial[1]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public static string GZipCompressString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                byte[] zippedData = Compress(rawData);
                return (string)(Convert.ToBase64String(zippedData));
            }
        }

        /// <summary>
        /// GZip压缩
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        static byte[] Compress(byte[] rawData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// 解压Sring 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string GetStringByString(string Value)
        {
            //DataSet ds = new DataSet();
            string CC = GZipDecompressString(Value);
            //System.IO.StringReader Sr = new System.IO.StringReader(CC);
            //ds.ReadXml(Sr);
            return CC;
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static DataSet GetDatasetByString(string Value)
        {
            DataSet ds = new DataSet();
            string CC = GZipDecompressString(Value);
            System.IO.StringReader Sr = new System.IO.StringReader(CC);
            ds.ReadXml(Sr);
            return ds;
        }


        /// <summary>
        /// 将传入的二进制字符串资料以GZip算法解压缩
        /// </summary>
        /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
        /// <returns>原始未压缩字符串</returns>
        public static string GZipDecompressString(string zippedString)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
                return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedData)));
            }
        }

        /// <summary>
        /// ZIP解压
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] zippedData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(zippedData);
            System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
            System.IO.MemoryStream outBuffer = new System.IO.MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();

        }
    }
	


}
