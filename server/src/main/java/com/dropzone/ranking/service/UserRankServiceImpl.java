package com.dropzone.ranking.service.impl;

import com.dropzone.matchstatistics.entity.UserStatisticsEntity;
import com.dropzone.ranking.dto.UserRankDTO;
import com.dropzone.ranking.repository.UserRankRepository;
import com.dropzone.ranking.service.UserRankService;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

@Service
public class UserRankServiceImpl implements UserRankService {

    private final UserRankRepository userRankRepository;

    public UserRankServiceImpl(UserRankRepository userRankRepository) {
        this.userRankRepository = userRankRepository;
    }

    @Override
    public List<UserRankDTO> getRankings(Optional<Integer> page) {
        int size = 10;  // 페이지 당 10개의 항목을 고정
        int pageNumber = page.map(p -> p - 1).orElse(0); // 페이지 값이 있으면 1을 빼고, 없으면 0 (즉, 1페이지가 첫 페이지가 되도록)

        // 로그 출력으로 페이지 번호 확인
        System.out.println("UserRankServiceImpl: 요청된 페이지 = " + pageNumber);

        Pageable pageable = PageRequest.of(pageNumber, size);
        return getPageableRankings(pageable);
    }

    private List<UserRankDTO> getPageableRankings(Pageable pageable) {
        // 1. 전체 유저 데이터를 점수로 정렬하여 가져옴
        List<UserStatisticsEntity> allRankings = userRankRepository.findAllByOrderByRankingPointsDescTotalWinsDesc();

        List<UserRankDTO> result = new ArrayList<>();
        int currentRank = 1;
        int lastRank = 1;
        int lastRankingPoints = allRankings.get(0).getRankingPoints(); // 첫 번째 유저의 랭킹 점수 저장

        // 2. 전체 유저의 순위를 계산
        for (int i = 0; i < allRankings.size(); i++) {
            UserStatisticsEntity user = allRankings.get(i);

            // 현재 유저와 이전 유저의 점수가 같으면 같은 순위 유지
            if (user.getRankingPoints() != lastRankingPoints) {
                currentRank = i + 1; // 새로운 점수가 나타날 때만 순위 증가
            }

            result.add(new UserRankDTO(user.getUserId(), user.getRankingPoints(), user.getTotalWins(), currentRank));

            lastRankingPoints = user.getRankingPoints(); // 마지막 점수 업데이트
            lastRank = currentRank; // 마지막 등수 업데이트
        }

        // 3. 해당 페이지의 데이터를 반환 (페이지 요청에 맞게 잘라서 반환)
        int start = pageable.getPageNumber() * pageable.getPageSize();
        int end = Math.min(start + pageable.getPageSize(), result.size());

        return result.subList(start, end); // 요청한 페이지의 데이터를 반환
    }




    @Override
    public UserRankDTO getUserRank(int userId) {
        UserStatisticsEntity user = userRankRepository.findUserStatisticsByUserId(userId);

        if (user == null) {
            throw new IllegalArgumentException("유저를 찾을 수 없습니다.");
        }

        // 유저의 랭킹 계산
        int userRank = userRankRepository.findUserRank(user.getRankingPoints(), user.getTotalWins());

        // UserRankDTO로 반환
        return new UserRankDTO(user.getUserId(), user.getRankingPoints(), user.getTotalWins(), userRank);
    }
}
