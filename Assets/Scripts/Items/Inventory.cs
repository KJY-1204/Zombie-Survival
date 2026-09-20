// 칸 수가 아니라 총 무게로만 제한되는 인벤토리. UI를 알지 못하고 상태와 변경 이벤트만 공개한다
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public float maxWeight = 40f; // 이 무게를 넘으면 과적 상태가 된다

    private readonly List<ItemStack> stacks = new List<ItemStack>(); // 실제 보유 묶음들

    public IReadOnlyList<ItemStack> items => stacks; // 외부에서는 읽기만 가능
    public event Action onChanged; // 내용물이나 무게가 바뀔 때 발동

    public float totalWeight { get; private set; } // 현재 보유 중인 아이템의 총 무게
    public bool isOverweight => totalWeight > maxWeight; // 과적 여부

    // 아이템을 인벤토리에 넣는다
    // 칸 제한이 없으므로 항상 전량 들어가고, 최대 무게를 넘겨도 획득 자체는 막지 않는다
    // (과적 상태가 되어 이동속도만 느려진다)
    public void Add(ItemData data, int count = 1) {
        if (data == null || count <= 0)
        {
            return;
        }

        int remain = count;

        // 같은 아이템의 기존 묶음부터 채운다
        foreach (ItemStack stack in stacks)
        {
            if (remain <= 0)
            {
                break;
            }

            if (stack.data != data)
            {
                continue;
            }

            int added = Mathf.Min(stack.freeSpace, remain);
            stack.count += added;
            remain -= added;
        }

        // 기존 묶음에 다 못 넣은 나머지는 새 묶음으로 만든다
        while (remain > 0)
        {
            int added = Mathf.Min(data.maxStack, remain);
            stacks.Add(new ItemStack(data, added));
            remain -= added;
        }

        NotifyChanged();
    }

    // 아이템을 count개 덜어낸다
    // 보유 수량이 모자라면 아무것도 덜어내지 않고 false를 반환한다
    public bool Remove(ItemData data, int count = 1) {
        if (data == null || count <= 0 || CountOf(data) < count)
        {
            return false;
        }

        int remain = count;

        // 뒤쪽 묶음부터 덜어내야 제거 중에 인덱스가 밀리지 않는다
        for (int i = stacks.Count - 1; i >= 0 && remain > 0; i--)
        {
            if (stacks[i].data != data)
            {
                continue;
            }

            int removed = Mathf.Min(stacks[i].count, remain);
            stacks[i].count -= removed;
            remain -= removed;

            if (stacks[i].count <= 0)
            {
                stacks.RemoveAt(i);
            }
        }

        NotifyChanged();
        return true;
    }

    // 해당 아이템을 모두 합쳐 몇 개 가지고 있는지 반환
    public int CountOf(ItemData data) {
        int total = 0;

        foreach (ItemStack stack in stacks)
        {
            if (stack.data == data)
            {
                total += stack.count;
            }
        }

        return total;
    }

    // index번째 묶음의 아이템을 user에게 사용한다
    // 효과가 실제로 적용된 경우에만 1개를 소모한다
    public bool Use(int index, GameObject user) {
        if (index < 0 || index >= stacks.Count)
        {
            return false;
        }

        ConsumableItemData consumable = stacks[index].data as ConsumableItemData;

        // 소비 아이템이 아니거나 효과가 적용되지 않았다면 소모하지 않는다
        if (consumable == null || !consumable.Apply(user))
        {
            return false;
        }

        stacks[index].count--;

        if (stacks[index].count <= 0)
        {
            stacks.RemoveAt(index);
        }

        NotifyChanged();
        return true;
    }

    // 총 무게를 다시 계산하고 변경 이벤트를 발동
    private void NotifyChanged() {
        float sum = 0f;

        foreach (ItemStack stack in stacks)
        {
            sum += stack.totalWeight;
        }

        totalWeight = sum;

        if (onChanged != null)
        {
            onChanged();
        }
    }
}
