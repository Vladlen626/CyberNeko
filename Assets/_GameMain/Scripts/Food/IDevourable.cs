using System;
using UnityEngine;

public interface IDevourable
{
    event Action OnDevoured;
    int GetPoints();

    void Show();
    void Hide();
    bool IsActive();
}
