# Pfizer Privacy Content DB System

## To Do

- 중복되는 수집데이타 처리 방안 (서로 다른 채널에서 같은 데이터가 온 경우 처리 로직)
- PforceRX.txt -> PforceRXFiles -> N360Outbound -> N360Files -> Privacy
- 채널 수집 기능 즉시 시작 기능
- 사용되는 채널은 N360, MMS, GRV, PforceRX, hardcopy
- 병합로직
- 벌크업로드는 승인절차 없음
- 승인요청시 메일 알림 ( 다수의 승인자 가능하게 )
- 시스템 로그

## 병합로직

* IND_ID가 같은 경우 -> 최종 도달한 데이터로 덮어 씌움.
* IND_ID가 다르고 [이메일,모바일] 이 같은 경우 -> 최종 도달한 데이터로 덮어 씌움.
* IND_ID 가 없는 경우 [이메일,모바일] 이 같은 경우
* IND_ID 가 없는 완전 신규 데이터



## 테스트케이스

1. PforceRX 채널로 부터 데이터 입력: 신규데이터(3) 
2. N360 채널로 부터 데이터 입력 : 신규데이터(2)
3. N360 채널로  데이터 전송 : 전체
4. N360 채널로 부터 데이터 입력
5. PforceRX 채널로 부터 데이터 입력: 중복데이터(3) 
6. N360 채널로 부터 데이터 입력 : 중복데이터(2)
7. N360 채널로  데이터 전송 : 전체
8. N360 채널로 부터 데이터 입력
9. PforceRX 채널로 데이터 전송 : IND_ID 가 있는 것.

## 병합로직

PforceRX -> PCMS -> N360 -> PCMS -> PforceRX

|      | PCMS | PforceRX       | N360            |
| ---- | ---- | -------------- | --------------- |
|      |      |                | SOURCE          |
|      |      |                | ACTION          |
|      |      |                | WKP_ID          |
|      |      |                | WKP_EXT_ID      |
|      |      | IND_ID         | IND_ID          |
|      |      |                | IND_EXT_ID      |
|      |      |                | ACT_STATUS      |
|      |      |                | WKP_NAME        |
|      |      |                | WKP_TEL         |
|      |      |                | WKP_FAX         |
|      |      |                | ZIP             |
|      |      |                | PROVINCE        |
|      |      |                | CITY            |
|      |      |                | STREET          |
|      |      |                | FULL_ADDR       |
|      |      |                | IND_SP          |
|      |      |                | TITLE           |
|      |      |                | IND_LASTNAME    |
|      |      |                | IND_FIRSTNAME   |
|      |      |                | INDFULL_NAME    |
|      |      |                | GENDER          |
|      |      | CONSENT_EMAIL  | EMAIL           |
|      |      | CONSENT_MOBILE | MOBILE          |
|      |      | CONSENT_STATUS | CONSENT_STATUS  |
|      |      | CONSENT_DATE   | CONSENT_DATE    |
|      |      | CONSENT_SOURCE | CONSENT_SOURCE  |
|      |      |                | CONSENT_TYPE    |
|      |      |                | CONSENT_VERSION |
|      |      | EXTRACT_DATE   |                 |
|      |      | COUNTRY_CD     |                 |
|      |      |                |                 |


## 로그관련
- Batches : 스케줄에 따른 로그
- Collection : 채널별 파일처리 내역


## Spec
.net version >= 4.5
asp.net mvc 5
ms-sql
iis7

## 관련 시스템
N360의 키로 계정-사람

- MMS : 계정
- GRV : 계정
- PforceRX  : 사람
- N360 (Inbound) = ODSK (Outbound) : 사람 기준
- MyPfizer


## Menu ( 메뉴 )
- Sign in ( 입장 )
- Sign out ( 퇴장 )
- Dashboard ( 대쉬보드 )
  -  승인요청건
  -  ​
  -  채널별 고객동의정보 수
  -  최근 채널 수집일시 : 채널별 변경 일시 -> 권한 유무 -> 내역보기로
  -  최근 고객동의정보 변경 내역 : 변경일시 및 수정자 -> 권한 유무 -> 내역보기로
  -  동의 만료 고객
  -  알림기능 : 동의서 버전업 / 기타 알림 ( 팝업 , 헤드배너  ...)
  -  and so so
- Agreement ( 동의서 관리 )
  - 동의서 등록 ( 업로드 ) / 수정 / 목록 / 사용불가 : 변경시 알림 발생
  - 출력?
- Privacy ( 고객동의정보 )
  - List ( 목록 )
    -  	search (by channel , by date, by expire , by duplication , etc)
         -  export to excel
         -  onekey / n360 info 보여주기
  - New ( 신규 ) : 해당 권한 소유자만 등록 가능
    - 	write (작성) & approval request (승인요청)
        - temp save (임시저장)
        - bulk excel upload
  - Detail ( 고객동의정보 보기 )

  - Modify ( 고객동의정보 수정 및 삭제 ) : 해당 권한 소유자만 등록 가능
    - Modify : 고객동의정보 수정
    - Delete : 고객동의정보 삭제 처리
  - Approval ( 승인 ) : 해당 권한 소유자만 등록 가능
    - Approval ( 승인 ) : 승인 요청된 고객동의정보에 대한 승인
    - Approval Log : 승인 내역
- System ( 설정 및 관리 )
  - Role ( 사용자 권한 설정 ) : 해당 권한 소유자만 등록 가능
  - Schedule ( 채널별 배치 작업 일정 설정 ) : 해당 권한 소유자만 등록 가능 , 매일 동기화 시점으로 변경 (분단위 x)
  - Channel Log ( 채널별 작업 내역 ) : 채널별 수집 내역
  - Privacy Log ( 고객동의정보 변경 내역 ) : 고객동의정보 변경 내역

## 기능
- Refresh : 변경정보를 해당 채널로 전송
- 수집시 검증 :  onekey 발급 및 onekey 정보 추가 , nucleus360 시스템도 동일


### Batch Schedule

- 로깅:[시작,종료,실패]
```
{
	job : 'SFTP JOB'
	status : [Begin, Fine, Error],
	createdate : Date
}
```

### Collect

- 로깅:[시작,정상완료,연결실패,파일다운로드실패,파싱실패,임포트실패]
- SFTP 접속 
- 파일목록가져오기
- 대상 검색
- 다운로드
- 파싱
- 임포트
- ​

### 사용자 및 권한

- 시스템관리자 : 전체
- 개인정보동의 관리자 : 시스템 메뉴 제외 모두
- 일반사용자 : 자신이 올린 데이타에 대한 대쉬보드 , 등록 ,목록 ,상세 , 승인 , 승인내역
- 옵져버 : 모든 데이타에 대한 대쉬보드

### 동의서 - 시스템 메뉴로 이전

MMS 에서만 사용 , 현재는 배제.














