package com.dropzone.ranking.controller;

import com.dropzone.ranking.dto.UserRankDTO;
import com.dropzone.ranking.service.UserRankService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;

@RestController
@RequestMapping("/api/rankings")
@Tag(name = "랭킹 API", description = "랭킹 조회 API")
public class UserRankController {

    private final UserRankService userRankService;

    public UserRankController(UserRankService userRankService) {
        this.userRankService = userRankService;
    }

    // 페이지 번호만 받아서 10개씩 반환
    @GetMapping
    @Operation(summary = "랭킹", description = "특정 페이지의 유저 랭킹 반환")
    public List<UserRankDTO> getRankings(@RequestParam Optional<Integer> page) {
        return userRankService.getRankings(page);
    }

    // 특정 유저의 랭킹 정보 반환
    @GetMapping("/{userId}")
    @Operation(summary = "특정 유저 랭킹", description = "특정 유저의 랭킹 정보 반환")
    public UserRankDTO getUserRank(@PathVariable int userId) {
        return userRankService.getUserRank(userId);
    }
}
