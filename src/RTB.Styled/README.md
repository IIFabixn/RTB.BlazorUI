# RTB.Styled

This package provides a component which can be used to style dynamically your components dynamically with the help of js.

````razor
@using RTB.Styled
@using RTB.Styled.Components
@using RTB.Styled.Helper

<Styled Context="ClassName">
    <Background Color="@RTBColors.Red" />
    <Size FullHeight FullWidth MaxHeight="@SizeUnit.Rem(24)" />

    <!-- Your Component -->
    <MyComponent Class="@ClassName" />
</Styled>