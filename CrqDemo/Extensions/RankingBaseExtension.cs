//using System;
//using PublicUse.BLL;

//namespace PublicUse.Rank
//{
//    /// <summary>
//    /// RankingBase 扩展类
//    /// </summary>
//    public static class RankingBaseExtension
//    {
//        /// <summary>
//        /// 获取个人排名说明(不在榜单内返回-1)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <returns>[排名,分数]</returns>
//        public static string GetMyRankInfo<T>(this RankingBase<T> source, string key) where T : Utils.RankModel, new()
//        {
//            var ranking = source.GetRankingByKey(key, out var rankModel);
//            if (ranking > 0)
//                return $"[{ranking},{rankModel.Score}]";
//            else
//                return "[-1,0]";
//        }

//        /// <summary>
//        /// 获取个人排名说明(不在榜单内返回10000+)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <returns>[排名,分数]</returns>
//        public static string GetMyRankInfoWithTenThousand<T>(this RankingBase<T> source, string key) where T : Utils.RankModel, new()
//        {
//            var ranking = source.GetRankingByKey(key, out var rankModel);
//            if (ranking > 0)
//                return $"[{ranking},{rankModel.Score}]";
//            else
//                return $"[\"{source.MaxLength}+\",0]";
//        }

//        /// <summary>
//        /// 获取个人排名说明(不在榜单内返回-1)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="additionalFunc">附加信息</param>
//        /// <returns>[排名,分数,{0}]</returns>
//        public static string GetMyRankInfo<T>(this RankingBase<T> source, string key, Func<T, object> additionalFunc, string defaultAdditionVal = "\"\"") where T : Utils.RankModel, new()
//        {
//            var ranking = source.GetRankingByKey(key, out var rankModel);
//            if (ranking > 0)
//                return $"[{ranking},{rankModel.Score},{additionalFunc(rankModel)}]";
//            else
//                return $"[-1,0,{defaultAdditionVal}]";
//        }

//        /// <summary>
//        /// 获取个人排名说明(不在榜单内返回暂无名次)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="additional">附加信息</param>
//        /// <returns>[排名说明,分数,{0}]</returns>
//        public static string GetMyRankInfoDesc<T>(this RankingBase<T> source, string key, string additional = "{0}") where T : Utils.RankModel, new()
//        {
//            var ranking = source.GetRankingByKey(key, out var rankModel);
//            if (ranking > 0)
//                return string.Format("[\"{0}\",{1},\"{2}\"]", ranking, rankModel.Score, additional);
//            else
//                return string.Format("[\"{0}\",{1},\"{2}\"]", InternationalInfo.Instance.暂无排名, 0, additional);
//        }

//        /// <summary>
//        /// 获取个人排名说明(不在榜单内返回暂无名次)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="additionalFunc">附加信息</param>
//        /// <returns>[排名说明,分数,{0}]</returns>
//        public static string GetMyRankInfoDesc<T>(this RankingBase<T> source, string key, Func<T, object> additionalFunc) where T : Utils.RankModel, new()
//        {
//            var ranking = source.GetRankingByKey(key, out var rankModel);
//            if (ranking > 0)
//                return string.Format("[\"{0}\",{1},\"{2}\"]", ranking, rankModel.Score, additionalFunc(rankModel));
//            else
//                return string.Format("[\"{0}\",{1},\"{2}\"]", InternationalInfo.Instance.暂无排名, 0, null);
//        }

//        /// <summary>
//        /// 获取个人排行说明(不在榜单内返回暂无名次)
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="userID"></param>
//        /// <param name="areaType"></param>
//        /// <returns>排名说明</returns>
//        public static string GetMyRankDesc<T>(this RankingBase<T> source, string key) where T : Utils.RankModel, new()
//        {
//            var myRank = source.GetRankingByKey(key);
//            if (myRank > 0)
//                return myRank.ToString();
//            else
//                return InternationalInfo.Instance.暂无排名;
//        }

//    }
//}
