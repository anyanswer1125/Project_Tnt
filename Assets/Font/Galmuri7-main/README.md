# 갈무리7

[배포처 바로가기](https://galmuri.quiple.dev/#%EB%8B%A4%EC%9A%B4%EB%A1%9C%EB%93%9C)

&nbsp;

## 웹 폰트

사용하는 `font-family`의 이름은 `Galmuri7`입니다.

### HTML

```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/gh/fonts-archive/Galmuri7/Galmuri7.css" type="text/css"/>
```

### CSS `@Import`

```css
@import url('https://cdn.jsdelivr.net/gh/fonts-archive/Galmuri7/Galmuri7.css');
```

### CSS `@font-face`

```css
@font-face {
    font-family: 'Galmuri7';
    font-weight: normal;
    font-style: normal;
    font-display: swap;
    src: url('https://cdn.jsdelivr.net/gh/fonts-archive/Galmuri7/Galmuri7.woff2') format('woff2'),
         url('https://cdn.jsdelivr.net/gh/fonts-archive/Galmuri7/Galmuri7.woff') format('woff'),
         url('https://cdn.jsdelivr.net/gh/fonts-archive/Galmuri7/Galmuri7.otf') format('opentype'),
         url('https://cdn.jsdelivr.net/gh/fonts-archive/Galmuri7/Galmuri7.ttf') format('truetype');
}
```

&nbsp;

## 다이나믹 서브셋

웹폰트의 최적화를 위해 모던 브라우저에서는 글리프를 여러개로 나누어 필요한 부분만 동적으로 파싱하는 다이나믹 서브셋을 제공합니다. 폰트의 용량이 부담된다면 아래 코드를 사용하는 걸 추천합니다.

### HTML

```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/gh/fonts-archive/Galmuri7/subsets/Galmuri7-dynamic-subset.css" type="text/css"/>
```

### CSS

```css
@import url('https://cdn.jsdelivr.net/gh/fonts-archive/Galmuri7/subsets/Galmuri7-dynamic-subset.css');
```

&nbsp;

## font-family

어느 브라우저나 시스템 환경에서도 동일한 폰트가 적용되어야 한다면 아래와 같이 구성하는 걸 추천합니다. `-apple-system`과 `BlinkMacSystemFont`는 맥, `Segoe UI`는 윈도우, `Roboto`는 안드로이드의 기본 폰트입니다.



```css
font-family: "Galmuri7", -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Oxygen, Ubuntu, Cantarell, "Open Sans", "Helvetica Neue", sans-serif;
```

&nbsp;

## 라이선스

라이선스는 언제든지 변경될 수 있습니다. 변경사항을 확인하려면 배포처를 방문해 주세요.

```
Copyright © 2021, Lee Minseo (itoupluk427@gmail.com), with reserved font name “Galmuri”.
 
Galmuri는 SIL 오픈 폰트 라이선스 1.1에 따라 사용할 수 있으며, 폰트가 자체적으로 판매되지 않는 한 자유롭게 사용·연구·수정·재배포할 수 있습니다. 또한 어떠한 경우에도 저작권자는 계약·불법 행위 또는 기타 계약의 조치로 인한 일반·특수·간접·부수·결과적 손해를 포함하여 어떠한 청구·손해 또는 기타 책임도 지지 않습니다.
 
Galmuri를 수정하여 파생된 이차적 저작물 폰트는 이름에 ‘Galmuri’를 사용할 수 없으며, 다른 유형의 라이선스로 배포할 수 없습니다.
```
