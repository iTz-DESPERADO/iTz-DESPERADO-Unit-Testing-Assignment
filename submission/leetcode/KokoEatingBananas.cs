// LeetCode 875 - Koko Eating Bananas
// Submit this file ALONE on LeetCode (C#).
public class Solution
{
    public int MinEatingSpeed(int[] piles, int h)
    {
        int low = 1;
        int high = 0;

        foreach (int pile in piles)
        {
            if (pile > high)
                high = pile;
        }

        while (low < high)
        {
            int mid = low + (high - low) / 2;
            long hoursNeeded = 0;

            foreach (int pile in piles)
            {
                hoursNeeded += (pile - 1) / mid + 1;

                if (hoursNeeded > h)
                    break;
            }

            if (hoursNeeded <= h)
                high = mid;
            else
                low = mid + 1;
        }

        return low;
    }
}
