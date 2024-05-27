//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ComUse
//{
//    public static class ActivityExtension
//    {
//        /// <summary>
//        /// 当前是否在领奖期
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <returns></returns>
//        public static bool IsDuringReceiveReward(this Activity source, ref string msg)
//        {
//            if (source == null)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动不存在;
//                return false;
//            }
//            var isDuring = PublicUse.BLL.InternationalHelper.During_Activity(source, ref msg);
//            if (!isDuring)
//            {
//                return false;
//            }
//            if (DateTime.Now < source.DateStart)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动尚未开始;
//                return false;
//            }
//            if (DateTime.Now <= source.DateEnd)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动正在进行中;
//                return false;
//            }
//            if (source.During_CalcuReward())
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.正在统计冲榜奖励请稍候;
//                return false;
//            }
//            return true;
//        }

//        /// <summary>
//        /// 当前时间在活动期间内（活动开始时间(DateStart)-活动结束时间期间(DateEnd)）
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="msg"></param>
//        /// <returns></returns>
//        public static bool IsDuringActivity(this Activity source, ref string msg)
//        {
//            if (source == null)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动不存在;
//                return false;
//            }
//            if (DateTime.Now < source.DateStart)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动尚未开始;
//                return false;
//            }
//            if (DateTime.Now > source.DateEnd)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动已结束;
//                return false;
//            }
//            return true;
//        }

//        /// <summary>
//        /// 当前时间在活动期间内（活动开始时间(PicStart)-活动结束时间期间(PicEnd)）
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="msg"></param>
//        /// <returns></returns>
//        public static bool IsDuringPic(this Activity source, ref string msg)
//        {
//            if (source == null)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动不存在;
//                return false;
//            }
//            var now = DateTime.Now;
//            if (now < source.PicStart)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动尚未开始;
//                return false;
//            }
//            if (now > source.PicEnd)
//            {
//                msg = PublicUse.BLL.InternationalInfo.Instance.活动已结束;
//                return false;
//            }
//            return true;
//        }
//    }
//}
