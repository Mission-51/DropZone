package com.dropzone.matchstatistics.service;

public class RankPointCalculator {

    // 랭크 포인트 계산 메소드
    public int calculateRankPoint(int playerCount, int rank) {
        int rankPoint = 0;

        switch (playerCount) {
            // 플레이어 수: 2
            case 2:
                switch (rank) {
                    // 등수: 1
                    case 1:
                        rankPoint = 2;
                        break;
                    // 등수: 2
                    case 2:
                        rankPoint = -2;
                        break;
                    // 예외 처리
                    default:
                        rankPoint = 0; // 예외 상황 처리
                        break;
                }
                break;

            // 플레이어 수: 3
            case 3:
                switch (rank) {
                    case 1:
                        rankPoint = 4;
                        break;
                    case 2:
                        rankPoint = 0;
                        break;
                    case 3:
                        rankPoint = -4;
                        break;
                    // 예외 처리
                    default:
                        rankPoint = 0;
                        break;
                }
                break;

            // 플레이어 수: 4
            case 4:
                switch (rank) {
                    case 1:
                        rankPoint = 6;
                        break;
                    case 2:
                        rankPoint = 2;
                        break;
                    case 3:
                        rankPoint = -2;
                        break;
                    case 4:
                        rankPoint = -6;
                        break;
                    // 예외 처리
                    default:
                        rankPoint = 0;
                        break;
                }
                break;

            // 플레이어 수: 5
            case 5:
                switch (rank) {
                    case 1:
                        rankPoint = 8;
                        break;
                    case 2:
                        rankPoint = 4;
                        break;
                    case 3:
                        rankPoint = 0;
                        break;
                    case 4:
                        rankPoint = -4;
                        break;
                    case 5:
                        rankPoint = -8;
                        break;
                    // 예외 처리
                    default:
                        rankPoint = 0;
                        break;
                }
                break;

            // 플레이어 수: 6
            case 6:
                switch (rank) {
                    case 1:
                        rankPoint = 10;
                        break;
                    case 2:
                        rankPoint = 6;
                        break;
                    case 3:
                        rankPoint = 2;
                        break;
                    case 4:
                        rankPoint = -2;
                        break;
                    case 5:
                        rankPoint = -6;
                        break;
                    case 6:
                        rankPoint = 0;
                        break;
                    // 예외 처리
                    default:
                        rankPoint = 0;
                        break;
                }
                break;
            default:
                rankPoint = 10; // 지원하지 않는 플레이어 수에 대한 처리
                break;
        }
        return rankPoint;
    }
}
