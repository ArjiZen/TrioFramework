using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bingosoft.TrioFramework.Component.Excel.NPOI
{
    /// <summary>
    /// 表头集合扩展
    /// </summary>
    public static class WorkHeadCollectionExtension
    {
        /// <summary>
        /// 将表头树转为二维坐标
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// <remarks>
        /// 思路：
        /// 先通过Colspan属性的值计算出第一行表头的二维数组列索引
        /// 然后从首行表头开始递归遍历子表头
        /// 子表头的二维数组列索引为父表头的二维数组列索引+当前表头在同级Children的索引
        /// </remarks>
        public static NPOI.WorkHead[][] Get2DArray(this WorkHeadCollection collection)
        {
            // 计算出总列数
            var cellCount = collection.Cast<NPOI.WorkHead>().Sum(p => p.ColSpan);
            var array = new List<NPOI.WorkHead[]> { new NPOI.WorkHead[cellCount] };
            // 定位首行列索引
            var prevColumnIndex = 0;
            for (int columnIndex = 0; columnIndex < collection.Count; columnIndex++)
            {
                var cell = collection[columnIndex] as NPOI.WorkHead;
                if (cell == null)
                    continue;

                array[0][columnIndex == 0 ? columnIndex : prevColumnIndex] = cell;

                EachChildren(ref array, cell.Children, 1, prevColumnIndex);
                prevColumnIndex += cell.ColSpan;
            }
            return array.ToArray();
        }

        /// <summary>
        /// 递归遍历子表头
        /// </summary>
        /// <param name="array">二维数组</param>
        /// <param name="collection">表头集合</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="aColumnIndex">父表头在二维数组中的列索引</param>
        private static void EachChildren(ref List<NPOI.WorkHead[]> array,
            WorkHeadCollection collection, int rowIndex, int aColumnIndex)
        {
            for (int oColumnIndex = 0; oColumnIndex < collection.Count; oColumnIndex++)
            {
                var child = collection[oColumnIndex];
                // 定位坐标并赋值
                SetElementToArray(ref array, collection, rowIndex, oColumnIndex, aColumnIndex);
                if (child.Children != null && child.Children.Count > 0)
                {
                    // 递归子表头
                    EachChildren(ref array, child.Children, rowIndex + 1, aColumnIndex + oColumnIndex);
                }
            }
        }

        /// <summary>
        /// 根据行列坐标设置二维数组的值
        /// </summary>
        /// <param name="array">二维数组</param>
        /// <param name="collection">表头集合</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="oColumnIndex">当前表头在同级集合中的索引</param>
        /// <param name="parentAColumnIndex">父级表头在二维数组中的列索引</param>
        private static void SetElementToArray(ref List<NPOI.WorkHead[]> array,
            WorkHeadCollection collection, int rowIndex, int oColumnIndex, int parentAColumnIndex)
        {
            if (array.Count <= rowIndex)
            {
                array.Add(new NPOI.WorkHead[array[rowIndex - 1].Length]);
            }
            array[rowIndex][parentAColumnIndex + oColumnIndex] = (NPOI.WorkHead)collection[oColumnIndex];
        }

    }

    /// <summary>
    /// Excel表头
    /// </summary>
    public class WorkHead : Excel.WorkHead
    {
        /// <summary>
        /// 行合并数
        /// </summary>
        public int ColSpan
        {
            get
            {
                return (this.Children == null || this.Children.Count == 0) ? 1 : this.Children.Cast<NPOI.WorkHead>().Sum(child => child.ColSpan);
            }
        }
    }
}
