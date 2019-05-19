using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BUS
{
    static public class RewriteUrl
    {
        static public String GetAliasLink(this String unicode)
        {
            unicode = unicode.ToLower();
            unicode = Regex.Replace(unicode, "[áàảãạăắằẳẵặâấầẩẫậ]", "a");
            unicode = Regex.Replace(unicode, "[óòỏõọôồốổỗộơớờởỡợ]", "o");
            unicode = Regex.Replace(unicode, "[éèẻẽẹêếềểễệ]", "e");
            unicode = Regex.Replace(unicode, "[íìỉĩị]", "i");
            unicode = Regex.Replace(unicode, "[úùủũụưứừửữự]", "u");
            unicode = Regex.Replace(unicode, "[ýỳỷỹỵ]", "y");
            unicode = Regex.Replace(unicode, "[đ]", "d");
            //unicode = Regex.Replace(unicode, "[-\\s+/]+", "-");
            unicode = Regex.Replace(unicode, "\\W+", "-"); //Nếu bạn muốn thay dấu khoảng trắng thành dấu "_" hoặc dấu cách " " thì thay kí tự bạn muốn vào đấu "-"
            return unicode;
        }

        static public String RemoveUnicode(this String unicode)
        {
            if (!string.IsNullOrEmpty(unicode))
            {
                unicode = unicode.ToLower();
                unicode = Regex.Replace(unicode, "[áàảãạăắằẳẵặâấầẩẫậ]", "a");
                unicode = Regex.Replace(unicode, "[óòỏõọôồốổỗộơớờởỡợ]", "o");
                unicode = Regex.Replace(unicode, "[éèẻẽẹêếềểễệ]", "e");
                unicode = Regex.Replace(unicode, "[íìỉĩị]", "i");
                unicode = Regex.Replace(unicode, "[úùủũụưứừửữự]", "u");
                unicode = Regex.Replace(unicode, "[ýỳỷỹỵ]", "y");
                unicode = Regex.Replace(unicode, "[đ]", "d");
                //unicode = Regex.Replace(unicode, "[-\\s+/]+", "-");
                unicode = Regex.Replace(unicode, "\\W+", " "); //Nếu bạn muốn thay dấu khoảng trắng thành dấu "_" hoặc dấu cách " " thì thay kí tự bạn muốn vào đấu "-"
            }
            return unicode;
        }
    }
}
