package com.dropzone.ranking.service.impl;

import com.dropzone.matchstatistics.entity.UserStatisticsEntity;
import com.dropzone.ranking.dto.UserRankDTO;
import com.dropzone.ranking.repository.UserRankRepository;
import com.dropzone.ranking.service.UserRankService;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

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

        return IntStream.range(0, rankingPage.getContent().size())
                .mapToObj(i -> {
                    UserStatisticsEntity user = rankingPage.getContent().get(i);
                    return new UserRankDTO(user.getUserId(), user.getRankingPoints(), user.getTotalWins(),
                            pageable.getPageNumber() * pageable.getPageSize() + i + 1);
                })
                .collect(Collectors.toList());
    }
}
