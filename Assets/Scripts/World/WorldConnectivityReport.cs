// 시작 지점에서 월드가 얼마나 닿는지에 대한 검사 결과
public class WorldConnectivityReport {
    public int reachableCount; // 시작 칸에서 갈 수 있는 칸 수
    public int reachableRoads; // 그중 도로 칸 수
    public int reachablePoi; // 그중 POI 칸 수
    public int totalRoads; // 월드 전체의 도로 칸 수
    public int totalPoi; // 월드 전체의 POI 칸 수

    // 도로망이 전부 이어져 있고 모든 POI에 갈 수 있으면 통과
    public bool isValid => reachableRoads == totalRoads && reachablePoi == totalPoi;

    public override string ToString() {
        return $"도달 가능 {reachableCount}칸 | 도로 {reachableRoads}/{totalRoads}"
            + $" | POI {reachablePoi}/{totalPoi} | 통과={isValid}";
    }
}
