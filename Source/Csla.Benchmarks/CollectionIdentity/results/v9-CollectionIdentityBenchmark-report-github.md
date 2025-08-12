```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800X 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.304
  [Host]               : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2
  .NET 9.0             : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  .NET Framework 4.6.2 : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256
  .NET Framework 4.7.2 : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256
  .NET Framework 4.8   : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256


```
| Method | Job                  | Runtime              | NumberOfItems | Mean             | Error          | StdDev         | Ratio | RatioSD |
|------- |--------------------- |--------------------- |-------------- |-----------------:|---------------:|---------------:|------:|--------:|
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **1**             |         **29.45 μs** |       **0.505 μs** |       **0.394 μs** |  **1.28** |    **0.02** |
| Create | .NET 9.0             | .NET 9.0             | 1             |         23.04 μs |       0.282 μs |       0.313 μs |  1.00 |    0.02 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 1             |         69.77 μs |       0.350 μs |       0.311 μs |  3.03 |    0.04 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 1             |         69.49 μs |       0.340 μs |       0.302 μs |  3.02 |    0.04 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 1             |         69.19 μs |       0.249 μs |       0.233 μs |  3.00 |    0.04 |
|        |                      |                      |               |                  |                |                |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **10**            |         **68.55 μs** |       **0.909 μs** |       **1.010 μs** |  **1.19** |    **0.03** |
| Create | .NET 9.0             | .NET 9.0             | 10            |         57.57 μs |       0.694 μs |       0.926 μs |  1.00 |    0.02 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 10            |        168.80 μs |       0.697 μs |       0.652 μs |  2.93 |    0.05 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 10            |        166.85 μs |       0.529 μs |       0.495 μs |  2.90 |    0.05 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 10            |        164.63 μs |       0.664 μs |       0.622 μs |  2.86 |    0.05 |
|        |                      |                      |               |                  |                |                |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **100**           |        **437.32 μs** |       **5.198 μs** |       **7.287 μs** |  **1.08** |    **0.03** |
| Create | .NET 9.0             | .NET 9.0             | 100           |        405.12 μs |       8.069 μs |      11.045 μs |  1.00 |    0.04 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 100           |      1,171.25 μs |       3.648 μs |       3.413 μs |  2.89 |    0.07 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 100           |      1,163.60 μs |       3.024 μs |       2.828 μs |  2.87 |    0.07 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 100           |      1,136.14 μs |       4.341 μs |       4.061 μs |  2.81 |    0.07 |
|        |                      |                      |               |                  |                |                |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **1000**          |      **6,863.75 μs** |      **48.827 μs** |      **45.673 μs** |  **1.11** |    **0.01** |
| Create | .NET 9.0             | .NET 9.0             | 1000          |      6,175.50 μs |      64.712 μs |      54.037 μs |  1.00 |    0.01 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 1000          |     23,000.04 μs |     313.796 μs |     293.525 μs |  3.72 |    0.06 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 1000          |     22,995.17 μs |     359.375 μs |     336.159 μs |  3.72 |    0.06 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 1000          |     22,126.28 μs |     350.239 μs |     327.614 μs |  3.58 |    0.06 |
|        |                      |                      |               |                  |                |                |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **10000**         |    **359,875.30 μs** |   **5,520.308 μs** |   **4,309.894 μs** |  **1.13** |    **0.03** |
| Create | .NET 9.0             | .NET 9.0             | 10000         |    319,742.46 μs |   6,274.912 μs |   8,376.828 μs |  1.00 |    0.04 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 10000         |    862,254.15 μs |  13,428.578 μs |  12,561.100 μs |  2.70 |    0.08 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 10000         |    858,410.73 μs |  15,028.564 μs |  14,760.059 μs |  2.69 |    0.08 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 10000         |    847,137.34 μs |  12,880.246 μs |  12,048.190 μs |  2.65 |    0.08 |
|        |                      |                      |               |                  |                |                |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **100000**        | **26,929,510.69 μs** | **211,958.518 μs** | **198,266.129 μs** |  **1.19** |    **0.01** |
| Create | .NET 9.0             | .NET 9.0             | 100000        | 22,542,351.26 μs | 155,043.768 μs | 137,442.262 μs |  1.00 |    0.01 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 100000        | 64,631,273.33 μs | 593,727.077 μs | 555,372.677 μs |  2.87 |    0.03 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 100000        | 65,546,452.35 μs | 361,826.482 μs | 338,452.716 μs |  2.91 |    0.02 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 100000        | 66,263,671.51 μs | 407,437.703 μs | 381,117.481 μs |  2.94 |    0.02 |
