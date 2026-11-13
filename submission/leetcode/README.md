# Part C — LeetCode: Binary Search

## Public LeetCode profile

**Profile:** `https://leetcode.com/u/uODsjF4zTG/`

## Accepted submissions

| Problem | Problem page | Accepted submission | Source file | Screenshot |
|---|---|---|---|---|
| 875. Koko Eating Bananas | https://leetcode.com/problems/koko-eating-bananas/ | `https://leetcode.com/submissions/detail/2195024954/` | `KokoEatingBananas.cs` | `screenshots/koko-accepted.png` |
| 33. Search in Rotated Sorted Array | https://leetcode.com/problems/search-in-rotated-sorted-array/ | `https://leetcode.com/submissions/detail/2195031856/` | `SearchInRotatedSortedArray.cs` | `screenshots/rotated-accepted.png` |

---

## Problem 875 — Koko Eating Bananas

### What am I binary-searching over?

I am **not** searching in the `piles` array I search possible integer eating speeds `k` (bananas per hour) The slowest possible speed is `low = 1` and the fastest speed I ever need is `high = max(piles)` because eating faster than the largest pile does not shorten any pile below one hour

### Why binary search applies

For any speed `k` the hours needed are :

```text
hours(k) = sum(ceil(pile / k)) for each pile
```

As `k` increases `hours(k)` never increases So speeds split into two parts :

 too slow (`hours > h`) then fast enough (`hours <= h`) I want **the first valid speed**

### Which half do I keep?

- If `hoursNeeded <= h`, `mid` is valid, but there might be a smaller valid speed. I keep it and search left: `high = mid`.
- If `hoursNeeded > h`, `mid` is too slow and all smaller speeds will also be too slow. I search right: `low = mid + 1`

I use `while (low < high)` When `low == high` the remaining number is the **minimum valid eating speed**. Hours accumulate in `long` to avoid overflow `(pile - 1) / mid + 1` is ceiling division for positive integer piles without using floating-point numbers

### Complexity

- **Time:** `O(n log M)`

- **Extra space:** `O(1)` 

### R5 — Hand trace of Example 1

**Input:** `piles = [3, 6, 7, 11]`, `h = 8`  
**Expected answer:** `4`

| Step | low | mid | high | Hours needed for mid | Decision | New interval |
|---|---:|---:|---:|---:|---|---|
| 1 | 1 | 6 | 11 | `1 + 1 + 2 + 2 = 6` | 6 ≤ 8: feasible, keep mid | `[1, 6]` |
| 2 | 1 | 3 | 6 | `1 + 2 + 3 + 4 = 10` | 10 > 8: too slow | `[4, 6]` |
| 3 | 4 | 5 | 6 | `1 + 2 + 2 + 3 = 8` | 8 ≤ 8: feasible | `[4, 5]` |
| 4 | 4 | 4 | 5 | `1 + 2 + 2 + 3 = 8` | 8 ≤ 8: feasible | `[4, 4]` |

**Stop:** `low = high = 4` so the minimum speed is **4 bananas/hour**

---

## Problem 33 — Search in Rotated Sorted Array

### What am I binary-searching over?

Here I search **array indices** from `low = 0` to `high = nums.Length - 1`. The array was originally sorted in ascending order and then rotated. All values are distinct according to the problem constraints.

### Why binary search applies

At each midpoint, **at least one half is sorted**:

- If `nums[low] <= nums[mid]`, the left half is sorted.
- Otherwise, the right half is sorted.

I check whether the target lies inside that sorted half If it does I keep it otherwise I discard it That removes roughly half the remaining indices each step

### Which half do I keep?

- If `nums[mid] == target` return `mid` immediately
- If the **left half** is sorted and `nums[low] <= target < nums[mid]` set `high = mid - 1` otherwise set `low = mid + 1`
- If the **right half** is sorted and `nums[mid] < target <= nums[high]` set `low = mid + 1` otherwise set `high = mid - 1`

The loop is `while (low <= high)` If the target is found, the method returns its index. If the loop ends with `low > high` no candidates remain so it returns `-1`

### Complexity

- **Time:** `O(log n)`

- **Extra space:** `O(1)`

### R5 — Hand trace of Example 1

**Input:** `nums = [4, 5, 6, 7, 0, 1, 2]`, `target = 0`  
**Expected answer:** `4`

| Step | low | mid | high | nums[mid] | Sorted half | Decision | New interval |
|---|---:|---:|---:|---:|---|---|---|
| 1 | 0 | 3 | 6 | 7 | Left `[4,5,6,7]` | Target 0 is not in `[4,7)`, discard left | `[4,6]` |
| 2 | 4 | 5 | 6 | 1 | Left `[0,1]` | Target 0 lies in `[0,1)`, search left | `[4,4]` |
| 3 | 4 | 4 | 4 | 0 | Single index | Found target | Return `4` |

**Stop:** The method returns **index 4**

---

