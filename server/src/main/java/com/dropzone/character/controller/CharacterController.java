package com.dropzone.character.controller;

import com.dropzone.character.dto.CharacterDTO;
import com.dropzone.character.service.CharacterService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/characters")
@CrossOrigin("*")
@RequiredArgsConstructor
@Slf4j
@Tag(name = "캐릭터 API", description = "캐릭터 생성, 조회, 수정, 삭제 관리")
public class CharacterController {

    private final CharacterService characterService;

    // 캐릭터 생성 API
    @Operation(summary = "캐릭터 생성 API", description = "새로운 캐릭터 생성")
    @PostMapping
    public ResponseEntity<?> createCharacter(@RequestBody CharacterDTO characterDTO) {
        log.info("캐릭터 생성 요청: {}", characterDTO.getCharacterName());
        try {
            characterService.createCharacter(characterDTO);
            log.info("캐릭터 생성 성공: {}", characterDTO.getCharacterName());
            return ResponseEntity.status(HttpStatus.CREATED).body("캐릭터 생성에 성공했습니다");
        } catch (Exception e) {
            log.error("캐릭터 생성 중 오류 발생: {}", characterDTO.getCharacterName(), e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 오류입니다");
        }
    }

    // 캐릭터 조회 API
    @Operation(summary = "캐릭터 조회 API", description = "캐릭터 ID로 캐릭터 정보 조회")
    @GetMapping("/{characterId}")
    public ResponseEntity<?> getCharacterById(
            @Parameter(description = "캐릭터 ID", required = true, example = "1")
            @PathVariable("characterId") Long characterId) {
        log.info("캐릭터 조회 요청: ID={}", characterId);
        try {
            CharacterDTO characterDTO = characterService.getCharacterById(characterId);
            if (characterDTO != null) {
                log.info("캐릭터 조회 성공: ID={}", characterId);
                return ResponseEntity.ok(characterDTO);
            } else {
                log.warn("캐릭터 조회 실패: 없는 ID={}", characterId);
                return ResponseEntity.status(HttpStatus.NOT_FOUND).body("해당 ID의 캐릭터가 없습니다");
            }
        } catch (Exception e) {
            log.error("캐릭터 조회 중 오류 발생: ID={}", characterId, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 오류입니다");
        }
    }

    // 캐릭터 수정 API
    @Operation(summary = "캐릭터 정보 수정 API", description = "캐릭터 정보 수정")
    @PutMapping("/{characterId}")
    public ResponseEntity<?> updateCharacter(
            @PathVariable("characterId") Long characterId,
            @RequestBody CharacterDTO characterDTO) {
        log.info("캐릭터 정보 수정 요청: ID={}", characterId);
        try {
            characterService.updateCharacter(characterId, characterDTO);
            log.info("캐릭터 정보 수정 성공: ID={}", characterId);
            return ResponseEntity.ok("캐릭터 정보가 수정되었습니다");
        } catch (Exception e) {
            log.error("캐릭터 정보 수정 중 오류 발생: ID={}", characterId, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 오류입니다");
        }
    }

    // 캐릭터 삭제 API
    @Operation(summary = "캐릭터 삭제 API", description = "캐릭터 ID로 캐릭터 삭제")
    @DeleteMapping("/{characterId}")
    public ResponseEntity<?> deleteCharacter(
            @Parameter(description = "캐릭터 ID", required = true, example = "1")
            @PathVariable("characterId") Long characterId) {
        log.info("캐릭터 삭제 요청: ID={}", characterId);
        try {
            characterService.deleteCharacter(characterId);
            log.info("캐릭터 삭제 성공: ID={}", characterId);
            return ResponseEntity.ok("캐릭터가 삭제되었습니다");
        } catch (Exception e) {
            log.error("캐릭터 삭제 중 오류 발생: ID={}", characterId, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 오류입니다");
        }
    }
}
