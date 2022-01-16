using System;

namespace YouiToolkit.Utils
{
	public static class SortAlgorithm
    {
		/// <summary>
		/// 快排
		/// </summary>
		/// <param name="a">源数组</param>
		/// <param name="left">起始下标</param>
		/// <param name="right">终止下标</param>
		public static void QuickSort(int[] a, int left, int right)
		{
			if (left >= right)
				return;
			int begin = left;
			int end = right;
			int keyi = end;
			int key = a[keyi];

			while (begin < end)
			{
				while (begin < end && a[begin] <= key)
					begin++;
				a[end] = a[begin];
				while (begin < end && a[end] >= key)
					end--;
				a[begin] = a[end];
			}
			a[end] = key;

			QuickSort(a, left, end - 1);
			QuickSort(a, end + 1, right);
		}

		/// <summary>
		/// 快排
		/// </summary>
		/// <param name="a">源数组</param>
		/// <param name="left">起始下标</param>
		/// <param name="right">终止下标</param>
		public static void QuickSort(float[] a, int left, int right)
		{
			if (left >= right)
				return;
			int begin = left;
			int end = right;
			int keyi = end;
			float key = a[keyi];

			while (begin < end)
			{
				while (begin < end && a[begin] <= key)
					begin++;
				a[end] = a[begin];
				while (begin < end && a[end] >= key)
					end--;
				a[begin] = a[end];
			}
			a[end] = key;

			QuickSort(a, left, end - 1);
			QuickSort(a, end + 1, right);
		}

		/// <summary>
		/// 快排
		/// </summary>
		/// <param name="a">源数组</param>
		/// <param name="left">起始下标</param>
		/// <param name="right">终止下标</param>
		public static void QuickSort(IComparable[] a, int left, int right)
		{
			if (left >= right)
				return;
			int begin = left;
			int end = right;
			int keyi = end;
			IComparable key = a[keyi];

			while (begin < end)
			{
				while (begin < end && a[begin].CompareTo(key) <= 0)
					begin++;
				a[end] = a[begin];
				while (begin < end && a[end].CompareTo(key) >= 0)
					end--;
				a[begin] = a[end];
			}
			a[end] = key;

			QuickSort(a, left, end - 1);
			QuickSort(a, end + 1, right);
		}
	}
}
