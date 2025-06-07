# Nora's UnityUtility

**노라가 Unity 프로젝트에서 자주 사용하는 공통 기능을 모아둔 유틸리티 패키지입니다.**

## 주요 기능

- **애니메이션 관리**: UI 요소 및 GameObject의 이동, 페이드, 슬라이드, 스케일 등 다양한 애니메이션 효과 지원
- **이징 함수 제공**: EaseOutQuad를 사용한 부드러운 애니메이션 효과 구현
- **코루틴 및 유틸리티 관리**: 간편하게 코루틴을 시작하고 정지할 수 있는 기능 포함
- **모듈화된 구조**: Helper 폴더 내에서 각 기능별로 파일이 분리되어 유지보수가 용이

## 설치 방법

### 1. Git URL을 통한 설치
Unity Package Manager를 사용하여 패키지를 설치할 수 있습니다.
1. Unity 에디터에서 **Window > Package Manager**를 엽니다.
2. 좌측 상단의 **“+”** 버튼을 클릭한 후 **“Add package from git URL...”** 을 선택합니다.
3. URL을 입력합니다:
https://github.com/KimNora07/UnityUtility.git

## 사용법 예제
```csharp
using UnityEngine;
using UnityEngine.UI;

// 네임스페이스 추가(중요!)
using KimNora07.UnityUtility;

public class Example : MonoBehaviour
{
    public RectTransform targetRect;
    public Image targetImage;

    void Start()
    {
        // UI 이동 애니메이션 예제
        UIAnimation.Move(targetRect, this, 1.0f, 0.0f, new Vector2(100, 100), 
            onPlay: () => Debug.Log("애니메이션 시작!"), 
            onComplete: () => Debug.Log("애니메이션 완료!"));

        // 페이드 애니메이션 예제
        UIAnimation.Fade(targetImage, this, Color.white, 0f, 1f, 1.0f, 0.0f,
            onPlay: () => Debug.Log("페이드 시작!"),
            onComplete: () => Debug.Log("페이드 완료!"));
    }
}
```

## 라이선스
이 패키지는 MIT 라이선스를 따릅니다. 자세한 내용은 LICENSE 파일을 참고하시기 바랍니다.

## 기타 정보
- 문서화: 자세한 API 문서와 사용법은 Documentation에서 확인해주세요!
- 이슈/문의: 문제가 있거나 제안이 있으시면 Issues를 꼭 활용해주세요!
- 저자: Nora
- Email: gimnora01@gmail.com
- Github: KimNora07

