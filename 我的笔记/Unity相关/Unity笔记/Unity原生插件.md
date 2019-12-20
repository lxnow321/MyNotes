# Unity原生插件

1. 创建一个C++ dll工程
2. 编写dll库
   xx.cpp
   
		extern "C" void TestSort(int a[], int length)
		{
			std:sort(a, a + length);
		}

	xx.h

		#define TESTDLLSORT_API __declspec(dllexport)
		extern "C" TESTDLLSORT_API void TestSort(int a[], int length);

3. Unity C#脚本中引用

	public class TestDll : MonoBehaviour
	{
		[DllImport("DllTest1", EntryPoint = "TestSort")]
		public static extern void TestSort(int[] a, int length);
		public int[] a = { 5, 6, 1, 3, 4 };

		void Start()
		{
			TestSort(a, a.Length);
			for(int i =0; i < a.Length; ++i)
			{
				Debug.LogError(a[i]);
			}
		}
	}