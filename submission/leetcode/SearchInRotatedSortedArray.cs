// LeetCode 33 - Search in Rotated Sorted Array
// Submit this file ALONE on LeetCode (C#).
public class Solution
{
    public int Search(int[] nums, int target)
    {
        int low = 0;
        int high = nums.Length - 1;

        while (low <= high)
        {
            int mid = low + (high - low) / 2;

            if (nums[mid] == target)
                return mid;

            if (nums[low] <= nums[mid])
            {
                // The left half is sorted.
                if (nums[low] <= target && target < nums[mid])
                    high = mid - 1;
                else
                    low = mid + 1;
            }
            else
            {
                // The right half is sorted.
                if (nums[mid] < target && target <= nums[high])
                    low = mid + 1;
                else
                    high = mid - 1;
            }
        }

        return -1;
    }
}
