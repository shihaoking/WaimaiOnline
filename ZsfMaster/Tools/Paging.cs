using System;
using System.Web.Mvc;

namespace ZsfProject.Tools
{
    public static class Paging
    {
        public static void ToPaging(int pageIndex, int dataCount, ControllerBase controller, int pageSize = 20)
        {
            int pagesCount = 1;
            int pageStartIndex = 1;
            int pageEndIndex = 0;
            controller.ViewBag.PageNowIndex = pageIndex;
            controller.ViewBag.PageSize = pageSize;
            controller.ViewBag.PagesCount = pagesCount = pageEndIndex = int.Parse(Math.Ceiling((double)dataCount / pageSize).ToString());

            if (pagesCount > 9)
            {
                //最多显示9个页码
                if (pageIndex > 5)
                {
                    pageStartIndex = pageIndex - 4;
                    if (pagesCount - pageIndex > 4)
                        pageEndIndex = pageIndex + 4;
                }
                else
                    pageEndIndex = 9;
            }

            controller.ViewBag.PageStartIndex = pageStartIndex;
            controller.ViewBag.PageEndIndex = pageEndIndex;
        }
    }
}