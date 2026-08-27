# Un

<p align="center">
  <img src="https://img.shields.io/badge/language-Un-blue" />
  <img src="https://img.shields.io/badge/status-development-orange" />
  <img src="https://img.shields.io/badge/license-MIT-green" />
</p>

---

## 주요 기능

* 간결하고 표현력 있는 문법
* 비동기 프로그래밍 기본 지원
* `wait`와 `go`를 통한 명시적인 비동기 제어
* 함수형 프로그래밍
* 패턴 매칭
* 구조 분해
* `use`를 이용한 모듈 시스템
* `using`을 이용한 자원 관리
* 문자열 보간 및 Tagged Template
* 클래스
* 기본 인자 및 키워드 인자

---

## Playground

설치 없이 웹 브라우저에서 Un을 직접 사용해 볼 수 있습니다.

**[Un Playground](https://unlangplay-8i2jhcwv.manus.space/)**

---

## 빠른 시작

### 변수

```un
x = 10
name = "Un"

value: int = 20
```

### 함수

```un
fn add(a, b) -> int
    -> a + b

result = add(10, 20)
```

### 비동기

```un
data = wait fetch("https://example.com")

go downloadFile("file.zip")
```

### 패턴 매칭

```un
match x {
    0: "zero",
    1: "one",
    _: "other",
}
```

---

## 사용법

### 모듈

```un
use net.http as http

res = wait http.get("https://example.com")

write(res.body)
```

### 구조 분해

```un
a, b = (1, 2)
x, y = point
```

### 자원 관리

```un
using file = open("log.txt")

file.write("hello")
```

### 클래스

```un
class Person
    name = ""

    fn greet()
        write(`Hi, I'm {name}`)
```

### 문자열 보간

```un
name = "Un"

write(`Hello, {name}!`)
```

Tagged Template도 지원합니다.

```un
html`<div>{user.name}</div>`
```

---

## 언어 문법

| 기능       | 문법                       |
| -------- | ------------------------ |
| 변수       | `x = 10`                 |
| 타입 지정    | `x: int = 10`            |
| 함수       | `fn add(a, b) -> a + b`  |
| 비동기 대기   | `wait expression`        |
| 백그라운드 실행 | `go expression`          |
| 모듈       | `use math`               |
| 모듈 별칭    | `use net.http as http`   |
| 패턴 매칭    | `match x { ... }`        |
| 구조 분해    | `a, b = (1, 2)`          |
| 자원 관리    | `using file = open(...)` |
| 문자열 보간   | `` `Hello, {name}` ``    |
| 클래스      | `class Person`           |

---

## 아키텍처

```text
소스 코드
    |
    v
  Lexer
    |
    v
  Parser
    |
    v
 AST / IR
    |
    v
 Runtime
    |
    v
Standard Library
```

Un은 Lexer, Parser, Runtime, Standard Library로 구성된 언어 처리 구조를 기반으로 개발되고 있습니다.

---

## License

MIT License
