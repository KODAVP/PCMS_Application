# PCMS

## To Do

### 수집

- 데이타 교환은 최신 상태의 데이터만 보낸다.
- N360 으로는 수집된 모든 데이터를 보낸다. 
- 다른 채널로는 N360에서 받은 데이터를 보낸다. (여기서 여러 채널이므로 모든 채널에게 동시에 보내거나 채널별 전송내역을 따로 체크.)
- 계정과 사람에 대한 판단은 오로지 N360이 한다. 채널에서 수집된 데이터는 N360을 통하기 전까지는 무효하다.

### 개인동의정보

* 대상별 계정 정보를 보여준다.
* 수집된 채널을 보여준다.
* 동의 상태 변경내역을 보여준다.

### 운영자/관리자

* ActiveDirectory 를 이용한 로그인 설정 ( 이메일 , 사용자명 )
* 메뉴별 운영자/관리자 권한 설정
* 운영자 활동에 대한 로깅 

### 동의서 관리

* 동의서를 등록/수정
* 신규 동의서 등록시 팝업을 통해 알림




```sequence
pforcerx->pcms: collect pforcerx datas
note right of pcms : pcmskey gen / gen n360data
pcms->n360: send data
n360->pcms: receive to n360datas
note right of pcms : update by pcmskey
pcms -> pforcerx : send datas from n360datas
```

