 스마트 팩토리 설비 실시간 모니터링 시스템 (FacilitySystemMonitoring)

C 기반의 비동기 TCP 통신 및 MVVM 패턴을 적용한 대시보드 프로젝트입니다.
실제 하드웨어(PLC/센서)의 거동을 모방한 시뮬레이터 서버와, 데이터를 실시간으로 시각화하는 WPF 클라이언트 아키텍처를 구축하여 데스크톱 애플리케이션 개발 역량을 증명합니다.



 1. 개발 환경 및 기술 스택
- Language: C#
- Framework: .NET WPF (클라이언트), .NET Console (시뮬레이터)
- Architecture: MVVM (Model-View-ViewModel) Pattern
- Libraries: System.Text.Json (표준 JSON 직렬화)


 2. 핵심 기능 및 구현 내용

 1단계: 가상 설비 시뮬레이터 (Server)
- 비동기 TCP 서버 구동: `AcceptTcpClientAsync`를 활용하여 UI 스레드 차단(Block) 없는 비동기 클라이언트 연결 대기 루프 구현.
- 실시간 데이터 생성 및 송신: 타이머를 통해 500ms 주기마다 설비 데이터(온도, 압력, 생산량)를 JSON 구조로 가공하여 라인 단위(`WriteLine`) 송신.
- 이상치(Error) 모사 로직: 난수 생성을 통해 5% 확률로 과열(`ERROR`) 및 과압(`WARN`) 상태를 인위적으로 연출하여 예외 처리 테스트 환경 구축.

 2단계: WPF 실시간 모니터링 대시보드 (Client)
- 정통 MVVM 아키텍처 패턴 준수: 코드 비하인드를 배제하고 `INotifyPropertyChanged` 및 `ICommand(RelayCommand)`를 순수 코드로 구현하여 관심사 분리(SoC) 실현.
- 백그라운드 데이터 수신 루프: `Task.Run` 기반의 멀티스레딩 환경에서 데이터를 지속 수신하며, 크로스 스레드 오류 방지를 위해 `Application.Current.Dispatcher.Invoke`를 적용하여 안전하게 UI 데이터 바인딩.


 3. 기술적 해결 과제 및 성과 (Troubleshooting)

 Q. 백그라운드 스레드 데이터 수신 시 UI가 멈추거나 크로스 스레드 예외가 발생하지 않았나요?
- 해결 방안: 통신 전담 서비스(`NetworkService`)의 수신 루프를 백그라운드 태스크로 완전히 격리하고, 수집된 JSON 데이터를 파싱하여 뷰모델 프로퍼티에 할당할 때 WPF 메인 UI 스레드의 `Dispatcher`를 경유하도록 설계하여 스레드 안정성을 확보했습니다.

 Q. 정통 MVVM 패턴을 고수하면서 얻은 이점은 무엇인가요?
- 해결 방안: 통신 로직, 데이터 모델, UI UI 컨트롤러를 완벽하게 분리하여 코드의 가독성을 높였으며, 향후 UI 프레임워크가 변경되거나 통신 프로토콜(예: MQTT)이 변경되더라도 상호 의존성 없이 독립적인 유지보수가 가능하도록 아키텍처를 설계했습니다.
