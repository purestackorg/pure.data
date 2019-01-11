
namespace Pure.Data.Validations.Validators
{
	using System;
    using Resources;
using System.Text.RegularExpressions;
    using Pure.Data.i18n;

	public class IDCardValidator : PropertyValidator
	{


        public IDCardValidator()
            : base(() => Messages.idcard_error)
        {
			 
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;

            var _String = context.PropertyValue.ToString();
            //if (_String.Length == 15)
            //{
            //    _String = CidUpdate(_String);
            //}
            //if (_String.Length == 18)
            //{
            //    string strResult = CheckCidInfo(_String);
            //    if (strResult == "非法地区" || strResult == "非法生日" || strResult == "非法证号")
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //    return false;
            //}   

            return ValidateIDNUM(_String);

			 
		}

        #region 中国身份证号码验证
        private string CheckCidInfo(string cid)
        {
            string[] aCity = new string[] { null, null, null, null, null, null, null, null, null, null, null, "北京", "天津", "河北", "山西", "内蒙古", null, null, null, null, null, "辽宁", "吉林", "黑龙江", null, null, null, null, null, null, null, "上海", "江苏", "浙江", "安微", "福建", "江西", "山东", null, null, null, "河南", "湖北", "湖南", "广东", "广西", "海南", null, null, null, "重庆", "四川", "贵州", "云南", "西藏", null, null, null, null, null, null, "陕西", "甘肃", "青海", "宁夏", "新疆", null, null, null, null, null, "台湾", null, null, null, null, null, null, null, null, null, "香港", "澳门", null, null, null, null, null, null, null, null, "国外" };
            double iSum = 0;
            string info = string.Empty;
            System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^\d{17}(\d|x)$");
            System.Text.RegularExpressions.Match mc = rg.Match(cid);
            if (!mc.Success)
            {
                return string.Empty;
            }
            cid = cid.ToLower();
            cid = cid.Replace("x", "a");
            if (aCity[int.Parse(cid.Substring(0, 2))] == null)
            {
                return "非法地区";
            }
            try
            {
                DateTime.Parse(cid.Substring(6, 4) + " - " + cid.Substring(10, 2) + " - " + cid.Substring(12, 2));
            }
            catch
            {
                return "非法生日";
            }
            for (int i = 17; i >= 0; i--)
            {
                iSum += (System.Math.Pow(2, i) % 11) * int.Parse(cid[17 - i].ToString(), System.Globalization.NumberStyles.HexNumber);
            }
            if (iSum % 11 != 1)
            {
                return ("非法证号");
            }
            else
            {
                return (aCity[int.Parse(cid.Substring(0, 2))] + "," + cid.Substring(6, 4) + "-" + cid.Substring(10, 2) + "-" + cid.Substring(12, 2) + "," + (int.Parse(cid.Substring(16, 1)) % 2 == 1 ? "男" : "女"));
            }
        }
        #endregion

        #region 身份证号码15升级为18位
        private string CidUpdate(string ShortCid)
        {
            char[] strJiaoYan = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
            int[] intQuan = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
            string strTemp;
            int intTemp = 0;

            strTemp = ShortCid.Substring(0, 6) + "19" + ShortCid.Substring(6);
            for (int i = 0; i <= strTemp.Length - 1; i++)
            {
                intTemp += int.Parse(strTemp.Substring(i, 1)) * intQuan[i];
            }
            intTemp = intTemp % 11;
            return strTemp + strJiaoYan[intTemp];
        }
        #endregion    



       
        public int Age
        {
            get
            {
                return GetAge(DateTime.Now);
            }
        }

        private DateTime _Brithday = DateTime.MinValue;
        public DateTime Brithday
        {
            get
            {
                return _Brithday;
            }
        }

        private bool _IsValidate = false;

        public bool Validate
        {
            get
            {
                return _IsValidate;
            }
        }

        private string _Sex;
        public string Sex
        {
            get
            {
                return _Sex;
            }
        }
        private int _Length = 0;
        public int Length
        {
            get
            {
                return _Length;
            }
        }

        private string _idnum = string.Empty;
 

        //public bool ValidateIDNUM(string IDNum)
        //{
        //    string idnum = IDNum;
        //    string _idnum = idnum.ToUpper();
        //    if (_idnum.Length != 15 && _idnum.Length != 18)
        //    {
        //        return false;
        //    }
        //    Regex reg = new Regex("^[1-9][0-9]{16}[0-9,X]$", RegexOptions.IgnoreCase);
        //    if (_idnum.Length == 15)
        //    {
        //        _Length = 15;
        //        reg = new Regex("^[1-9][0-9]{14}$", RegexOptions.IgnoreCase);
        //    }
        //    else
        //    {
        //        _Length = 18;
        //    }
        //    if (!reg.IsMatch(_idnum))
        //    {
        //        return false;
        //    }

        //    //地区码

        //    //生日
        //    string dt = string.Empty;
        //    if (_idnum.Length == 15)
        //    {
        //        dt = "19" + _idnum.Substring(6, 6);
        //    }
        //    else
        //    {
        //        dt = _idnum.Substring(6, 8);
        //    }
        //    try
        //    {
        //        dt = dt.Substring(0, 4) + "-" + dt.Substring(4, 2) + "-" + dt.Substring(6, 2);
        //        DateTime _tempDT = DateTime.Parse(dt);
        //        if (_tempDT == DateTime.MinValue || _tempDT == DateTime.MaxValue || _tempDT >= DateTime.Now)
        //            return false;
        //        _Brithday = _tempDT;
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    //性别
        //    int sexNum = 0;
        //    try
        //    {
        //        if (_idnum.Length == 15)
        //        {
        //            sexNum = int.Parse(_idnum.Substring(14, 1));
        //        }
        //        else
        //        {
        //            sexNum = int.Parse(_idnum.Substring(16, 1));
        //        }
        //        sexNum = sexNum % 2;
        //        if (sexNum == 0)
        //        {
        //            _Sex = "女";
        //        }
        //        else
        //            _Sex = "男";
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    //18位身份证的校验码
        //    if (_idnum.Length == 18)
        //    {
        //        int sum = 0;
        //        int[] tempArray1 = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
        //        string[] tempArray2 = new string[] { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
        //        string checkCode = _idnum.Substring(17, 1);

        //        try
        //        {
        //            for (int i = 0; i < 17; i++)
        //            {
        //                sum += int.Parse(_idnum.Substring(i, 1)) * tempArray1[i];
        //            }
        //            int index = sum % 11;
        //            if (checkCode != tempArray2[index])
        //            {
        //                return false;
        //            }
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        /// <summary>
        /// 验证大陆身份证
        /// </summary>
        /// <param name="idnum"></param>
        /// <returns></returns>
        public static bool ValidateIDNUM(string idnum)
        {
            string _idnum = idnum.ToUpper();
            if (_idnum.Length != 15 && _idnum.Length != 18)
            {
                return false;
            }
            Regex reg = new Regex("^[1-9][0-9]{16}[0-9,X]$", RegexOptions.IgnoreCase);
            if (_idnum.Length == 15)
            {
                reg = new Regex("^[1-9][0-9]{14}$", RegexOptions.IgnoreCase);
            }
            if (!reg.IsMatch(_idnum))
            {
                return false;
            }

            //地区码

            //生日
            string dt = string.Empty;
            if (_idnum.Length == 15)
            {
                dt = "19" + _idnum.Substring(6, 6);
            }
            else
            {
                dt = _idnum.Substring(6, 8);
            }
            try
            {
                dt = dt.Substring(0, 4) + "-" + dt.Substring(4, 2) + "-" + dt.Substring(6, 2);
                DateTime _tempDT = DateTime.Parse(dt);
                if (_tempDT == DateTime.MinValue || _tempDT == DateTime.MaxValue || _tempDT == DateTime.Now)
                    return false;
            }
            catch
            {
                return false;
            }

            //性别
            int sexNum = 0;
            try
            {
                if (_idnum.Length == 15)
                {
                    sexNum = int.Parse(_idnum.Substring(14, 1));
                }
                else
                {
                    sexNum = int.Parse(_idnum.Substring(16, 1));
                }
                sexNum = sexNum % 2;
            }
            catch
            {
                return false;
            }

            //18位身份证的校验码
            if (_idnum.Length == 18)
            {
                int sum = 0;
                int[] tempArray1 = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
                string[] tempArray2 = new string[] { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
                string checkCode = _idnum.Substring(17, 1);

                try
                {
                    for (int i = 0; i < 17; i++)
                    {
                        sum += int.Parse(_idnum.Substring(i, 1)) * tempArray1[i];
                    }
                    int index = sum % 11;
                    if (checkCode != tempArray2[index])
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public int GetAge(DateTime dt)
        {
            if (_Brithday == DateTime.MinValue || dt == DateTime.MinValue || dt < _Brithday)
                return 0;
            int ay = dt.Year;
            int by = _Brithday.Year;
            int am = dt.Month;
            int bm = _Brithday.Month;
            int ad = dt.Day;
            int b = _Brithday.Day;
            if (ay - by <= 0)
                return 0;
            if (am > bm)
                return ay - by;
            if (am < bm)
                return (ay - by) - 1;
            if (am == bm)
            {
                if (am >= bm)
                    return ay - by;
                else
                    return (ay - by) - 1;
            }
            return 0;
        }
        /// <summary>
        /// 15身份证转为18位身份证
        /// </summary>
        /// <param name="idnum15"></param>
        /// <returns></returns>
        public static string ChangeIdnum15To18(string idnum15)
        {
            string idnum18 = idnum15.Substring(0, 6) + "19" + idnum15.Substring(6, 9);
            int sum = 0;
            int[] tempArray1 = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            string[] tempArray2 = new string[] { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };

            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(idnum18.Substring(i, 1)) * tempArray1[i];
            }
            int index = sum % 11;
            idnum18 += tempArray2[index];

            return idnum18;
        }

        /// <summary>
        /// 18身份证转为15位身份证
        /// </summary>
        /// <param name="idnum18"></param>
        /// <returns></returns>
        public static string ChangeIdnum18To15(string idnum18)
        {
            string idnum15 = idnum18.Substring(0, 6) + idnum18.Substring(8, 9);

            return idnum15;
        }
	}
}