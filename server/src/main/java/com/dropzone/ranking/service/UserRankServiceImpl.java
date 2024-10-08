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
        Page<UserStatisticsEntity> rankingPage = userRankRepository.findAllRankings(pageable);

        // 로그 출력을 통한 디버깅
        System.out.println("UserRankServiceImpl: 요청된 페이지 = " + pageable.getPageNumber());

        // 이전 유저의 점수와 비교해 같은 점수인 경우 같은 순위로 표시
        List<UserStatisticsEntity> content = rankingPage.getContent();
        List<UserRankDTO> result = new ArrayList<>();

        // 첫 번째 페이지에서 1등부터 시작하고, 그 이후에는 이전 페이지의 마지막 등수를 유지
        int startRank = pageable.getPageNumber() * pageable.getPageSize() + 1; // 페이지에 따른 시작 순위 계산
        int currentRank = startRank; // 현재 페이지의 첫 번째 순위
        int lastRank = currentRank; // 마지막 유저의 등수 기록
        int lastRankingPoints = content.isEmpty() ? -1 : content.get(0).getRankingPoints(); // 첫 번째 유저의 랭킹 점수 저장

        for (int i = 0; i < content.size(); i++) {
            UserStatisticsEntity user = content.get(i);

            // 현재 유저와 이전 유저의 점수가 같으면 같은 순위로 표시 (같은 등수 유지)
            if (user.getRankingPoints() != lastRankingPoints) {
                currentRank = lastRank + 1; // 점수가 다르면 순위를 증가시킴
            }

            result.add(new UserRankDTO(user.getUserId(), user.getRankingPoints(), user.getTotalWins(), currentRank));

            lastRankingPoints = user.getRankingPoints(); // 마지막 점수 업데이트
            lastRank = currentRank; // 마지막 등수 업데이트
        }

        return result;
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
