//using System;
//using PublicUse.BLL;

//namespace ComUse
//{
//    /// <summary>
//    /// RankOverride 扩展类
//    /// </summary>
//    public static class RankOverrideExtension
//    {
//        /// <summary>
//        /// 获取个人排行说明(不在榜单内返回暂无名次)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="userID"></param>
//        /// <param name="areaType"></param>
//        /// <returns>排名说明</returns>
//        public static string GetMyRankDesc(this RankOverride source, int userID, int areaType)
//        {
//            var myRank = source.GetIndex(userID, areaType);
//            if (myRank > 0)
//                return myRank.ToString();
//            else
//                return InternationalInfo.Instance.暂无排名;
//        }

//        /// <summary>
//        /// 获取个人排名说明(不在榜单内返回暂无名次)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="userID">玩家ID</param>
//        /// <param name="additional">附加信息</param>
//        /// <returns>[排名说明,分数,{0}]</returns>
//        public static string GetMyRankInfoDesc(this RankOverride source, int userID, string additional = "{0}")
//        {
//            var (myRank, score) = source.GetMyRankData(userID);
//            if (myRank > 0)
//                return string.Format("[\"{0}\",{1},\"{2}\"]", myRank, score, additional);
//            else
//                return string.Format("[\"{0}\",{1},\"{2}\"]", InternationalInfo.Instance.暂无排名, 0, additional);
//        }

//        /// <summary>
//        /// 获取个人排名说明(不在榜单内返回暂无名次)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="userID">玩家ID</param>
//        /// <param name="areaType">玩家区服</param>
//        /// <param name="additional">附加信息</param>
//        /// <returns>[排名说明,分数,{0}]</returns>
//        public static string GetMyRankInfoDesc(this RankOverride source, int userID, int areaType, string additional = "{0}")
//        {
//            var (myRank, score) = source.GetMyRankData(userID, areaType);
//            if (myRank > 0)
//                return string.Format("[\"{0}\",{1},\"{2}\"]", myRank, score, additional);
//            else
//                return string.Format("[\"{0}\",{1},\"{2}\"]", InternationalInfo.Instance.暂无名次, 0, additional);
//        }        
//    }
//}
